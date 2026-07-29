using Chat.Api.Extensions;
using Chat.Application.DTOs;
using Chat.Application.Interfaces;
using Chat.Grpc;
using Grpc.Core;

namespace Chat.Api.Grpc;

public class GrpcUserManagementService(IUserManagementService userManagementService) : UserManagementService.UserManagementServiceBase
{
    public override async Task<UserResponse> Get(GetRequest request, ServerCallContext context)
    {
        var result = await userManagementService.Get(request.UserId, context.CancellationToken);
        
        return result.Match(
            user => new UserResponse
            {
                Id = user.ExternalId,
                Username = user.Username,
                Email = user.Email,
                IsEnabled = user.Enabled
            },
            errors => throw errors.ToRpcException()
        );
    }

    public override async Task<CreateResponse> Create(CreateRequest request, ServerCallContext context)
    {
        var createDto = new UserCreateDto(request.Username, request.Email, request.Password);

        var result = await userManagementService.Create(createDto, context.CancellationToken);

        return result.Match(
            id => new CreateResponse { UserId = id.Value },
            errors => throw errors.ToRpcException()
        );
    }

    public override async Task<UpdateResponse> Update(UpdateRequest request, ServerCallContext context)
    {
        var updateDto = new UserUpdateDto(request.Username, request.Email);

        var result = await userManagementService.Update(request.UserId, updateDto, context.CancellationToken);

        return result.Match(
            _ => new UpdateResponse{ Success = true },
            errors => throw errors.ToRpcException()
        );
    }

    public override async Task<DeleteResponse> Delete(DeleteRequest request, ServerCallContext context)
    {
        var result = await userManagementService.Delete(request.UserId, context.CancellationToken);

        return result.Match(
            _ => new DeleteResponse{ Success = true },
            errors => throw errors.ToRpcException()
        );
    }
}
