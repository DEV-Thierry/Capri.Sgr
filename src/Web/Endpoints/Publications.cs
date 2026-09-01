using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.Publications;
using Capri.Sgr.Domain.Constants;
using Capri.Sgr.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Capri.Sgr.Web.Endpoints;

/// <summary>Anonymous Portal público institucional read seam and protected editorial operations.</summary>
public sealed class Publications : IEndpointGroup
{
    public static string RoutePrefix => "/api/publications";
    public static void Map(RouteGroupBuilder group)
    {
        group.MapGet(Search).AllowAnonymous();
        group.MapGet(Get).AllowAnonymous();
        group.MapGet(ListEditorial, "editorial").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Consult));
        group.MapPost(CreateDraft, "editorial").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Maintain));
        group.MapPut(Revise, "editorial/{id:guid}").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Maintain));
        group.MapPost(SubmitForReview, "editorial/{id:guid}/submit-review").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Maintain));
        group.MapPost(ApproveReview, "editorial/{id:guid}/approve-review").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Decide));
        group.MapPost(RejectReview, "editorial/{id:guid}/reject-review").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Decide));
        group.MapPost(Schedule, "editorial/{id:guid}/schedule").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Decide));
        group.MapPost(Publish, "editorial/{id:guid}/publish").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Decide));
        group.MapPost(Archive, "editorial/{id:guid}/archive").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Decide));
        group.MapPost(PublishScheduled, "editorial/publish-scheduled").RequireAuthorization(AdministrativePermission.Policy(AdministrativeResources.InstitutionalPublications, AdministrativeActions.Decide));
    }
    [EndpointSummary("Search published institutional publications without authentication")]
    public static Task<Ok<PublicPublicationPage>> Search(PublicPublicationQueries queries, string? q, string? category, int page = 1, int pageSize = 20, CancellationToken ct = default) => SearchCore(queries, q, category, page, pageSize, ct);
    private static async Task<Ok<PublicPublicationPage>> SearchCore(PublicPublicationQueries queries, string? q, string? category, int page, int pageSize, CancellationToken ct) => TypedResults.Ok(await queries.SearchAsync(q, category, page, pageSize, ct));
    [EndpointSummary("Read a published institutional publication without authentication")]
    public static async Task<Results<Ok<PublicPublication>, NotFound>> Get(Guid id, PublicPublicationQueries queries, CancellationToken ct) => await queries.GetAsync(id, ct) is { } publication ? TypedResults.Ok(publication) : TypedResults.NotFound();
    public static async Task<Ok<IReadOnlyCollection<PublicationDetail>>> ListEditorial(IApplicationDbContext context, CancellationToken ct) => TypedResults.Ok<IReadOnlyCollection<PublicationDetail>>((await context.InstitutionalPublications.Include(item => item.Versions).OrderByDescending(item => item.CreatedAt).ToListAsync(ct)).Select(PublicationDetail.From).ToArray());
    public static async Task<Created<PublicationDetail>> CreateDraft(InstitutionalPublicationService service, CreatePublication request, CancellationToken ct) { var item = await service.CreateDraftAsync(request, ct); return TypedResults.Created($"/api/publications/editorial/{item.Id}", item); }
    public static Task<Ok<PublicationDetail>> Revise(Guid id, InstitutionalPublicationService service, EditPublication request, CancellationToken ct) => Ok(service.ReviseAsync(id, request, ct));
    public static Task<Ok<PublicationDetail>> SubmitForReview(Guid id, InstitutionalPublicationService service, ReasonRequest request, CancellationToken ct) => Ok(service.SubmitForReviewAsync(id, request.Reason, ct));
    public static Task<Ok<PublicationDetail>> ApproveReview(Guid id, InstitutionalPublicationService service, ReasonRequest request, CancellationToken ct) => Ok(service.ApproveReviewAsync(id, request.Reason, ct));
    public static Task<Ok<PublicationDetail>> RejectReview(Guid id, InstitutionalPublicationService service, RequiredReasonRequest request, CancellationToken ct) => Ok(service.RejectReviewAsync(id, request.Reason, ct));
    public static Task<Ok<PublicationDetail>> Schedule(Guid id, InstitutionalPublicationService service, ScheduleRequest request, CancellationToken ct) => Ok(service.ScheduleAsync(id, request.ScheduledFor, request.Reason, ct));
    public static Task<Ok<PublicationDetail>> Publish(Guid id, InstitutionalPublicationService service, ReasonRequest request, CancellationToken ct) => Ok(service.PublishAsync(id, request.Reason, ct));
    public static Task<Ok<PublicationDetail>> Archive(Guid id, InstitutionalPublicationService service, ReasonRequest request, CancellationToken ct) => Ok(service.ArchiveAsync(id, request.Reason, ct));
    public static async Task<Ok<int>> PublishScheduled(InstitutionalPublicationService service, CancellationToken ct) => TypedResults.Ok(await service.PublishScheduledAsync(ct));
    private static async Task<Ok<PublicationDetail>> Ok(Task<PublicationDetail> task) => TypedResults.Ok(await task);
}
public sealed record ReasonRequest(string? Reason);
public sealed record RequiredReasonRequest(string Reason);
public sealed record ScheduleRequest(DateTimeOffset ScheduledFor, string? Reason);
