using Capri.Sgr.Application.MemberApplications;
using Capri.Sgr.Application.MemberApplications.Commands;
using Capri.Sgr.Domain.Entities;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Capri.Sgr.Web.Endpoints;

public sealed class MembershipApplications : IEndpointGroup
{
    public static string RoutePrefix => "/api/membership-applications";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.MapPost(CreateDraft);
        groupBuilder.MapPost(Submit, "{applicationId:guid}/submit");
    }

    [EndpointSummary("Create a PF membership application draft")]
    public static async Task<Created<MembershipApplication>> CreateDraft(ISender sender, CreatePfMembershipApplicationDraftRequest request)
    {
        var application = await sender.Send(new CreatePfMembershipApplicationDraftCommand(request.TenantId, request.ApplicantId, request.Channel));
        return TypedResults.Created($"{RoutePrefix}/{application.Id}", application);
    }

    [EndpointSummary("Submit a PF membership application")]
    public static async Task<Ok<MembershipApplication>> Submit(ISender sender, Guid applicationId, SubmitPfMembershipApplicationRequest request) =>
        TypedResults.Ok(await sender.Send(new SubmitPfMembershipApplicationCommand(request.ToSubmission(applicationId))));
}

public sealed record CreatePfMembershipApplicationDraftRequest(string TenantId, string? ApplicantId, string Channel);
public sealed record SubmitPfMembershipApplicationRequest(string MembershipType, string Cpf, string FullName, DateOnly BirthDate, string Email, string PrimaryPhone, string CorrespondenceAddress, string IdentityDocument, string TermVersion, string AcceptanceActorId, string Channel)
{
    public PfMembershipApplicationSubmission ToSubmission(Guid applicationId) => new(applicationId, MembershipType, Cpf, FullName, BirthDate, Email, PrimaryPhone, CorrespondenceAddress, IdentityDocument, TermVersion, AcceptanceActorId, Channel);
}
