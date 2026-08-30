using Capri.Sgr.Application.Common.Interfaces;
using Capri.Sgr.Application.InternalUsers.Commands.CreateInternalUser;
using Capri.Sgr.Application.InternalUsers.Commands.UpdateInternalUserPermissions;
using Capri.Sgr.Application.InternalUsers.Queries.GetInternalUsers;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Capri.Sgr.Web.Endpoints;

public sealed class InternalUsers : IEndpointGroup
{
    public static string RoutePrefix => "/api/internal-users";

    public static void Map(RouteGroupBuilder groupBuilder)
    {
        groupBuilder.RequireAuthorization();
        groupBuilder.MapGet(GetInternalUsers);
        groupBuilder.MapPost(CreateInternalUser);
        groupBuilder.MapPut(UpdateInternalUserPermissions, "{userId}/permissions");
    }

    [EndpointSummary("List internal users")]
    public static async Task<Ok<IReadOnlyCollection<InternalUser>>> GetInternalUsers(ISender sender) => TypedResults.Ok(await sender.Send(new GetInternalUsersQuery()));

    [EndpointSummary("Create an internal user")]
    public static async Task<Created<InternalUser>> CreateInternalUser(ISender sender, CreateInternalUserCommand command)
    {
        var user = await sender.Send(command);
        return TypedResults.Created($"/api/internal-users/{user.Id}", user);
    }

    [EndpointSummary("Replace an internal user's administrative permissions")]
    public static async Task<NoContent> UpdateInternalUserPermissions(ISender sender, string userId, UpdateInternalUserPermissionsRequest request)
    {
        await sender.Send(new UpdateInternalUserPermissionsCommand(userId, request.Permissions));
        return TypedResults.NoContent();
    }
}

public sealed record UpdateInternalUserPermissionsRequest(IReadOnlyCollection<string> Permissions);
