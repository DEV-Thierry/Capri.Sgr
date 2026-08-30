using Capri.Sgr.Domain.Entities;
using NUnit.Framework;
using Shouldly;

namespace Capri.Sgr.Domain.UnitTests.Entities;

public class AssociatedDocumentTests
{
    [Test]
    public void VersionDoesNotParticipateInValidationUntilStorageAndAuditAreConfirmed()
    {
        var document = CreateDocument();
        var version = AddVersion(document);

        version.Status.ShouldBe(AssociatedDocumentVersionStatus.PendingSecureStorage);
        version.CanParticipateInValidation.ShouldBeFalse();
        Should.Throw<InvalidOperationException>(() => document.StartAnalysis(version.Id, "analyst-1", Now));

        document.ConfirmSecureStorageAndAudit(version.Id, "secure://dossier/1", Guid.NewGuid());

        version.Status.ShouldBe(AssociatedDocumentVersionStatus.Sent);
        version.IsSecurelyStored.ShouldBeTrue();
        version.CanParticipateInValidation.ShouldBeFalse();
    }

    [Test]
    public void ApprovalPreservesDecisionEvidenceAndMakesCurrentVersionValid()
    {
        var document = CreateDocument();
        var version = Confirm(AddVersion(document), document);

        document.StartAnalysis(version.Id, "analyst-1", Now.AddMinutes(1));
        document.Approve(version.Id, "analyst-1", Now.AddMinutes(2), "documento conferido");

        version.Status.ShouldBe(AssociatedDocumentVersionStatus.Approved);
        version.DecisionAuthorId.ShouldBe("analyst-1");
        version.DecidedAt.ShouldBe(Now.AddMinutes(2));
        version.Reason.ShouldBe("documento conferido");
        version.CanParticipateInValidation.ShouldBeTrue();
        document.HasApprovedCurrentVersion().ShouldBeTrue();
    }

    [Test]
    public void ResubmissionPreservesRejectedVersionAndReplacesCurrentVersionOnlyAfterConfirmation()
    {
        var document = CreateDocument();
        var first = Confirm(AddVersion(document), document);
        document.RequestResubmission(first.Id, "analyst-1", Now.AddMinutes(1), "imagem ilegível");

        var replacement = AddVersion(document, "arquivo-legivel.pdf");

        first.Status.ShouldBe(AssociatedDocumentVersionStatus.ReturnedForResubmission);
        first.IsCurrent.ShouldBeTrue();
        replacement.IsCurrent.ShouldBeFalse();
        document.CurrentVersion.ShouldBe(first);

        document.ConfirmSecureStorageAndAudit(replacement.Id, "secure://dossier/2", Guid.NewGuid());

        first.Status.ShouldBe(AssociatedDocumentVersionStatus.Superseded);
        first.IsCurrent.ShouldBeFalse();
        replacement.IsCurrent.ShouldBeTrue();
        document.CurrentVersion.ShouldBe(replacement);
        document.Versions.Count.ShouldBe(2);
    }

    [Test]
    public void DecisionsRequireReasonAndCannotBeChangedAfterTerminalDecision()
    {
        var document = CreateDocument();
        var version = Confirm(AddVersion(document), document);

        Should.Throw<ArgumentException>(() => document.Reject(version.Id, "analyst-1", Now, " "));
        document.Reject(version.Id, "analyst-1", Now, "documento vencido");

        Should.Throw<InvalidOperationException>(() => document.Approve(version.Id, "analyst-2", Now, "revisão"));
    }

    private static readonly DateTimeOffset Now = new(2026, 8, 30, 12, 0, 0, TimeSpan.Zero);

    private static AssociatedDocument CreateDocument() => new("dossier-42", "Identidade");

    private static AssociatedDocumentVersion AddVersion(AssociatedDocument document, string fileName = "identidade.pdf") =>
        document.AddVersion("author-1", Now, fileName, "application/pdf", 512, "sha256:abc", "primeiro envio");

    private static AssociatedDocumentVersion Confirm(AssociatedDocumentVersion version, AssociatedDocument document)
    {
        document.ConfirmSecureStorageAndAudit(version.Id, "secure://dossier/1", Guid.NewGuid());
        return version;
    }
}
