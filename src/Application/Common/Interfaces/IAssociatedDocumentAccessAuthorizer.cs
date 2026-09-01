namespace Capri.Sgr.Application.Common.Interfaces;

/// <summary>
/// Authorization seam for sensitive dossier operations. Implementations decide
/// which interested parties, authorized Responsáveis, or internal users may act.
/// </summary>
public interface IAssociatedDocumentAccessAuthorizer
{
    Task DemandAsync(AssociatedDocumentAccessRequest request, CancellationToken cancellationToken);
}

public sealed record AssociatedDocumentAccessRequest(
    string ActorId,
    string DossierId,
    AssociatedDocumentAccessAction Action);

public enum AssociatedDocumentAccessAction
{
    SubmitVersion,
    AnalyseVersion,
    View,
    Download
}
