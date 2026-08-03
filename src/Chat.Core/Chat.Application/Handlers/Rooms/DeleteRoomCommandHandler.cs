using Chat.Application.Commands.Rooms;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Rooms;

public class DeleteRoomCommandHandler(IRepository<ChatRoom, RoomId> roomRepository)
    : IRequestHandler<DeleteRoomCommand, ValueTask<Result<bool>>>
{
    public async ValueTask<Result<bool>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var roomId = RoomId.From(request.Id);
        var room = await roomRepository.GetById(roomId, cancellationToken);
        if (room is null)
        {
            return true;
        }
        
        return await roomRepository.Remove(room, cancellationToken);
    }
}
