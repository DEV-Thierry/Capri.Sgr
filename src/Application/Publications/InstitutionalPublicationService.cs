using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Domain.Entities;

namespace Capri.Sgr.Application.Publications;

public sealed class InstitutionalPublicationService(IApplicationDbContext context, IAuditStore auditStore, IOperationalClock clock, IUser user)
{
    public async Task<PublicationDetail> CreateDraftAsync(CreatePublication request, CancellationToken cancellationToken)
    {
        var actor = RequireActor(); var now = clock.GetCurrentInstant();
        var publication = new InstitutionalPublication(request.Category, request.Title, request.Summary, request.Body, actor, now, request.RequiresReview);
        context.InstitutionalPublications.Add(publication);
        await AuditAsync(publication, actor, "Rascunho criado", request.Reason, null, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return PublicationDetail.From(publication);
    }

    public async Task<PublicationDetail> ReviseAsync(Guid id, EditPublication request, CancellationToken cancellationToken)
    {
        var publication = await FindAsync(id, cancellationToken); var actor = RequireActor();
        var before = Snapshot(publication); publication.Revise(request.Title, request.Summary, request.Body, actor, clock.GetCurrentInstant());
        await AuditAsync(publication, actor, "Rascunho revisado", request.Reason, before, cancellationToken); await context.SaveChangesAsync(cancellationToken); return PublicationDetail.From(publication);
    }

    public async Task<PublicationDetail> SubmitForReviewAsync(Guid id, string? reason, CancellationToken cancellationToken) => await TransitionAsync(id, "Submetida para revisão", reason, publication => publication.SubmitForReview(), cancellationToken);
    public async Task<PublicationDetail> ApproveReviewAsync(Guid id, string? reason, CancellationToken cancellationToken) { var actor = RequireActor(); return await TransitionAsync(id, "Revisão aprovada", reason, publication => publication.ApproveReview(actor, clock.GetCurrentInstant(), reason), cancellationToken, actor); }
    public async Task<PublicationDetail> RejectReviewAsync(Guid id, string reason, CancellationToken cancellationToken) { var actor = RequireActor(); return await TransitionAsync(id, "Revisão rejeitada", reason, publication => publication.RejectReview(actor, clock.GetCurrentInstant(), reason), cancellationToken, actor); }
    public async Task<PublicationDetail> ScheduleAsync(Guid id, DateTimeOffset scheduledFor, string? reason, CancellationToken cancellationToken) => await TransitionAsync(id, "Agendada", reason, publication => publication.Schedule(scheduledFor), cancellationToken);
    public async Task<PublicationDetail> PublishAsync(Guid id, string? reason, CancellationToken cancellationToken) => await TransitionAsync(id, "Publicada", reason, publication => publication.Publish(clock.GetCurrentInstant()), cancellationToken);
    public async Task<PublicationDetail> ArchiveAsync(Guid id, string? reason, CancellationToken cancellationToken) => await TransitionAsync(id, "Arquivada", reason, publication => publication.Archive(clock.GetCurrentInstant()), cancellationToken);

    public async Task<int> PublishScheduledAsync(CancellationToken cancellationToken)
    {
        var now = clock.GetCurrentInstant(); var actor = RequireActor();
        var scheduled = await context.InstitutionalPublications.Where(item => item.Status == InstitutionalPublicationStatus.Scheduled && item.ScheduledFor <= now).ToListAsync(cancellationToken);
        foreach (var publication in scheduled) { var before = Snapshot(publication); publication.Publish(now); await AuditAsync(publication, actor, "Publicação agendada executada", null, before, cancellationToken); }
        await context.SaveChangesAsync(cancellationToken); return scheduled.Count;
    }

    private async Task<PublicationDetail> TransitionAsync(Guid id, string action, string? reason, Action<InstitutionalPublication> transition, CancellationToken cancellationToken, string? actor = null)
    {
        var publication = await FindAsync(id, cancellationToken); actor ??= RequireActor(); var before = Snapshot(publication); transition(publication);
        await AuditAsync(publication, actor, action, reason, before, cancellationToken); await context.SaveChangesAsync(cancellationToken); return PublicationDetail.From(publication);
    }
    private async Task<InstitutionalPublication> FindAsync(Guid id, CancellationToken ct) => await context.InstitutionalPublications.Include(item => item.Versions).SingleOrDefaultAsync(item => item.Id == id, ct) ?? throw new KeyNotFoundException("Publicação institucional não encontrada.");
    private async Task AuditAsync(InstitutionalPublication publication, string actor, string action, string? reason, string? before, CancellationToken ct) => await auditStore.AppendAsync(new AuditRecord(actor, clock.GetCurrentInstant(), "administrative-api", "Publicação institucional", publication.Id.ToString(), action, reason, before, Snapshot(publication)), ct);
    private string RequireActor() => user.Id ?? throw new UnauthorizedAccessException();
    private static string Snapshot(InstitutionalPublication publication) => System.Text.Json.JsonSerializer.Serialize(new { status = publication.Status.ToString(), version = publication.CurrentVersion.Number, title = publication.CurrentVersion.Title });
}

public sealed class PublicPublicationQueries(IApplicationDbContext context, IOperationalClock clock)
{
    public async Task<PublicPublicationPage> SearchAsync(string? query, string? category, int page, int pageSize, CancellationToken ct)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 1, 50); var now = clock.GetCurrentInstant();
        var items = context.InstitutionalPublications.AsNoTracking().Include(item => item.Versions).Where(item => item.Status == InstitutionalPublicationStatus.Published && item.PublishedAt <= now);
        if (!string.IsNullOrWhiteSpace(category)) items = items.Where(item => item.Category == category.Trim());
        if (!string.IsNullOrWhiteSpace(query)) { var term = query.Trim().ToUpperInvariant(); items = items.Where(item => item.Versions.Any(version => version.Title.ToUpper().Contains(term) || version.Summary.ToUpper().Contains(term) || version.Body.ToUpper().Contains(term))); }
        var total = await items.CountAsync(ct); var publications = await items.OrderByDescending(item => item.PublishedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new(Publications: publications.Select(PublicPublication.From).ToArray(), Total: total, Page: page, PageSize: pageSize);
    }
    public async Task<PublicPublication?> GetAsync(Guid id, CancellationToken ct) { var now = clock.GetCurrentInstant(); var item = await context.InstitutionalPublications.AsNoTracking().Include(p => p.Versions).SingleOrDefaultAsync(p => p.Id == id && p.Status == InstitutionalPublicationStatus.Published && p.PublishedAt <= now, ct); return item is null ? null : PublicPublication.From(item); }
}

public sealed record CreatePublication(string Category, string Title, string Summary, string Body, bool RequiresReview, string? Reason);
public sealed record EditPublication(string Title, string Summary, string Body, string? Reason);
public sealed record PublicationDetail(Guid Id, string Category, string Status, bool RequiresReview, string? ReviewerId, DateTimeOffset? ScheduledFor, DateTimeOffset? PublishedAt, IReadOnlyCollection<PublicationVersion> Versions) { public static PublicationDetail From(InstitutionalPublication p) => new(p.Id, p.Category, p.Status.ToString(), p.RequiresReview, p.ReviewerId, p.ScheduledFor, p.PublishedAt, p.Versions.OrderBy(v => v.Number).Select(v => new PublicationVersion(v.Number,v.Title,v.Summary,v.Body,v.AuthorId,v.CreatedAt)).ToArray()); }
public sealed record PublicationVersion(int Number, string Title, string Summary, string Body, string AuthorId, DateTimeOffset CreatedAt);
public sealed record PublicPublication(Guid Id, string Category, string Title, string Summary, string Body, DateTimeOffset PublishedAt) { public static PublicPublication From(InstitutionalPublication p) => new(p.Id,p.Category,p.CurrentVersion.Title,p.CurrentVersion.Summary,p.CurrentVersion.Body,p.PublishedAt!.Value); }
public sealed record PublicPublicationPage(IReadOnlyCollection<PublicPublication> Publications, int Total, int Page, int PageSize);
