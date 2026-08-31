namespace Capri.Sgr.Domain.Entities;

/// <summary>Editorial content maintained by the Equipe da associação for the Portal público institucional.</summary>
public sealed class InstitutionalPublication
{
    private readonly List<InstitutionalPublicationVersion> _versions = [];
    private InstitutionalPublication() { }

    public InstitutionalPublication(string category, string title, string summary, string body, string authorId, DateTimeOffset createdAt, bool requiresReview)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(category);
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);
        Id = Guid.NewGuid();
        Category = category.Trim();
        AuthorId = authorId;
        CreatedAt = createdAt;
        RequiresReview = requiresReview;
        Status = InstitutionalPublicationStatus.Draft;
        AddVersion(title, summary, body, authorId, createdAt);
    }

    public Guid Id { get; private set; }
    public string Category { get; private set; } = null!;
    public string AuthorId { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
    public bool RequiresReview { get; private set; }
    public InstitutionalPublicationStatus Status { get; private set; }
    public string? ReviewerId { get; private set; }
    public DateTimeOffset? ReviewedAt { get; private set; }
    public string? ReviewReason { get; private set; }
    public DateTimeOffset? ScheduledFor { get; private set; }
    public DateTimeOffset? PublishedAt { get; private set; }
    public DateTimeOffset? ArchivedAt { get; private set; }
    public IReadOnlyCollection<InstitutionalPublicationVersion> Versions => _versions.AsReadOnly();
    public InstitutionalPublicationVersion CurrentVersion => _versions.MaxBy(version => version.Number)!;

    public void Revise(string title, string summary, string body, string authorId, DateTimeOffset revisedAt)
    {
        EnsureEditable();
        ArgumentException.ThrowIfNullOrWhiteSpace(title);
        ArgumentException.ThrowIfNullOrWhiteSpace(body);
        ArgumentException.ThrowIfNullOrWhiteSpace(authorId);
        AddVersion(title, summary, body, authorId, revisedAt);
        ReviewerId = null;
        ReviewedAt = null;
        ReviewReason = null;
        Status = InstitutionalPublicationStatus.Draft;
    }

    public void SubmitForReview()
    {
        EnsureStatus(InstitutionalPublicationStatus.Draft);
        if (!RequiresReview) throw new InvalidOperationException("This category permits direct publication.");
        Status = InstitutionalPublicationStatus.InReview;
    }

    public void ApproveReview(string reviewerId, DateTimeOffset reviewedAt, string? reason)
    {
        EnsureStatus(InstitutionalPublicationStatus.InReview);
        ArgumentException.ThrowIfNullOrWhiteSpace(reviewerId);
        if (reviewerId == AuthorId) throw new InvalidOperationException("The creator and reviewer must be distinct for this category.");
        ReviewerId = reviewerId;
        ReviewedAt = reviewedAt;
        ReviewReason = reason;
        Status = InstitutionalPublicationStatus.Draft;
    }

    public void RejectReview(string reviewerId, DateTimeOffset reviewedAt, string reason)
    {
        EnsureStatus(InstitutionalPublicationStatus.InReview);
        ArgumentException.ThrowIfNullOrWhiteSpace(reviewerId);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        if (reviewerId == AuthorId) throw new InvalidOperationException("The creator and reviewer must be distinct for this category.");
        ReviewerId = reviewerId;
        ReviewedAt = reviewedAt;
        ReviewReason = reason;
        Status = InstitutionalPublicationStatus.Draft;
    }

    public void Schedule(DateTimeOffset scheduledFor)
    {
        EnsurePublishable();
        if (scheduledFor <= CreatedAt) throw new ArgumentOutOfRangeException(nameof(scheduledFor), "The schedule must be after creation.");
        ScheduledFor = scheduledFor;
        Status = InstitutionalPublicationStatus.Scheduled;
    }

    public void Publish(DateTimeOffset publishedAt)
    {
        if (Status is not (InstitutionalPublicationStatus.Draft or InstitutionalPublicationStatus.Scheduled))
            throw new InvalidOperationException("Only a draft or scheduled publication can be published.");
        EnsureReviewCompleted();
        if (Status == InstitutionalPublicationStatus.Scheduled && ScheduledFor > publishedAt)
            throw new InvalidOperationException("A scheduled publication cannot be published before its scheduled time.");
        PublishedAt = publishedAt;
        ScheduledFor = null;
        Status = InstitutionalPublicationStatus.Published;
    }

    public void Archive(DateTimeOffset archivedAt)
    {
        EnsureStatus(InstitutionalPublicationStatus.Published);
        ArchivedAt = archivedAt;
        Status = InstitutionalPublicationStatus.Archived;
    }

    private void AddVersion(string title, string summary, string body, string authorId, DateTimeOffset createdAt) =>
        _versions.Add(new InstitutionalPublicationVersion(_versions.Count + 1, title.Trim(), summary?.Trim() ?? string.Empty, body, authorId, createdAt));

    private void EnsureEditable()
    {
        if (Status is InstitutionalPublicationStatus.Published or InstitutionalPublicationStatus.Archived)
            throw new InvalidOperationException("Published or archived publications are immutable; create a new publication to change public content.");
    }

    private void EnsurePublishable()
    {
        if (Status != InstitutionalPublicationStatus.Draft) throw new InvalidOperationException("Only a draft can be scheduled.");
        EnsureReviewCompleted();
    }

    private void EnsureReviewCompleted()
    {
        if (RequiresReview && (ReviewerId is null || ReviewedAt is null))
            throw new InvalidOperationException("An approved review is required before publication.");
    }

    private void EnsureStatus(InstitutionalPublicationStatus expected)
    {
        if (Status != expected) throw new InvalidOperationException($"Expected publication status {expected}, but was {Status}.");
    }
}

public sealed class InstitutionalPublicationVersion
{
    private InstitutionalPublicationVersion() { }
    internal InstitutionalPublicationVersion(int number, string title, string summary, string body, string authorId, DateTimeOffset createdAt)
    {
        Id = Guid.NewGuid(); Number = number; Title = title; Summary = summary; Body = body; AuthorId = authorId; CreatedAt = createdAt;
    }
    public Guid Id { get; private set; }
    public int Number { get; private set; }
    public string Title { get; private set; } = null!;
    public string Summary { get; private set; } = null!;
    public string Body { get; private set; } = null!;
    public string AuthorId { get; private set; } = null!;
    public DateTimeOffset CreatedAt { get; private set; }
}

public enum InstitutionalPublicationStatus { Draft, InReview, Scheduled, Published, Archived }
