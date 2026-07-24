using Chat.Api.Extensions;
using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Chat.Api.Handlers;

public static class UserManagementEndpointHandler
{
    public static async Task<IResult> Get(
        [FromRoute] string externalId,
        IUserManagementService userManagementService,
        CancellationToken cancellationToken)
    {
        var result = await userManagementService.Get(externalId, cancellationToken);

        return result.Match(
            Results.Ok,
            errors => errors.ToProblem()
        );
    }

    public static async Task<IResult> Create(
        [FromBody] UserCreateDto createDto,
        IUserManagementService userManagementService,
        CancellationToken cancellationToken)
    {
        var result = await userManagementService.Create(createDto, cancellationToken);

        return result.Match(
            externalId => Results.Ok(new
            {
                externalId = externalId.Value
            }),
            errors => errors.ToProblem()
        );
    }

    public static async Task<IResult> Update(
        [FromRoute] string externalId,
        [FromBody] UserUpdateDto updateDto,
        IUserManagementService userManagementService,
        CancellationToken cancellationToken)
    {
        var result = await userManagementService.Update(externalId, updateDto, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblem()
        );
    }

    public static async Task<IResult> Delete(
        [FromRoute] string externalId,
        IUserManagementService userManagementService,
        CancellationToken cancellationToken)
    {
        var result = await userManagementService.Delete(externalId, cancellationToken);

        return result.Match(
            _ => Results.NoContent(),
            errors => errors.ToProblem()
        );
    }
}
