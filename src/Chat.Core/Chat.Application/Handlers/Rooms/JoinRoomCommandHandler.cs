using Chat.Application.Commands.Rooms;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Rooms;

public class JoinRoomCommandHandler(
    IRepository<ChatRoom, RoomId> roomRepository,
    IConnectionTracker connectionTracker)
    : IRequestHandler<JoinRoomCommand, ValueTask<Result<bool>>>
{
    public async ValueTask<Result<bool>> Handle(JoinRoomCommand request, CancellationToken cancellationToken)
    {
        var roomResult = (await roomRepository.GetById(RoomId.From(request.RoomId), cancellationToken))
            .OnError(Error.NotFound("ChatRoom.NotFound", "Chat room was not found"));
        
        if (roomResult.IsFailure)
        {
            return roomResult.PrimaryError;
        }
        
        var connection = new UserConnection(
            request.ConnectionId,
            request.UserId,
            request.Username,
            request.RoomId
        );
        
        await connectionTracker.SetConnection(connection, cancellationToken);

        return true;
    }
}
