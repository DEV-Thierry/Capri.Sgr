using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class InstitutionalPublicationTests
{
    private static readonly DateTimeOffset Now = new(2026, 8, 30, 9, 0, 0, TimeSpan.FromHours(-3));

    [Test]
    public void ReviewGovernanceRequiresDistinctReviewerBeforeSchedulingAndPreservesVersions()
    {
        var publication = New(requiresReview: true);
        publication.Revise("Assembleia geral", "Resumo atualizado", "Conteúdo revisado", "author-1", Now.AddMinutes(1));
        publication.SubmitForReview();
        Should.Throw<InvalidOperationException>(() => publication.ApproveReview("author-1", Now.AddMinutes(2), null));
        publication.ApproveReview("reviewer-2", Now.AddMinutes(2), "conteúdo conferido");
        publication.Schedule(Now.AddDays(1));

        publication.Status.ShouldBe(InstitutionalPublicationStatus.Scheduled);
        publication.Versions.Count.ShouldBe(2);
        publication.CurrentVersion.Number.ShouldBe(2);
        publication.ReviewerId.ShouldBe("reviewer-2");
    }

    [Test]
    public void PublishedContentCannotChangeAndCanBeArchived()
    {
        var publication = New(requiresReview: false);
        publication.Publish(Now.AddMinutes(1));

        Should.Throw<InvalidOperationException>(() => publication.Revise("Outro", "Resumo", "Conteúdo", "author-1", Now.AddMinutes(2)));
        publication.Archive(Now.AddMinutes(3));
        publication.Status.ShouldBe(InstitutionalPublicationStatus.Archived);
    }

    [Test]
    public void ScheduledPublicationCannotBePublishedEarly()
    {
        var publication = New(requiresReview: false);
        publication.Schedule(Now.AddDays(1));
        Should.Throw<InvalidOperationException>(() => publication.Publish(Now.AddHours(1)));
    }

    private static InstitutionalPublication New(bool requiresReview) => new("Notícias", "Título", "Resumo", "Conteúdo", "author-1", Now, requiresReview);
}
