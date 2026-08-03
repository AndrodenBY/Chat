using Chat.Application.Commands.Rooms;
using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Rooms;

public class UpdateRoomCommandHandler(IRepository<ChatRoom, RoomId> roomRepository)
    : IRequestHandler<UpdateRoomCommand, ValueTask<Result<ChatRoomDto>>>
{
    public async ValueTask<Result<ChatRoomDto>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var roomResult = (await roomRepository.GetById(RoomId.From(request.Id), cancellationToken))
                   .OnError(Error.NotFound("ChatRoom.NotFound", "Chat room was not found"));

        if (roomResult.IsFailure)
        {
            return roomResult.PrimaryError;
        }
        
        var room = roomResult.Value;
        var updateResult = room.UpdateDetails(request.Name, request.Description);
        if (updateResult.IsFailure)
        {
            return updateResult.PrimaryError;
        }
        
        if (updateResult)
        {
            room.Touch(DateTimeOffset.UtcNow);
            var repositoryUpdate = await roomRepository.Update(room, cancellationToken);
            
            return new ChatRoomDto(
                (string)repositoryUpdate.Name, 
                (string)repositoryUpdate.Description!
            );
        }
        
        return new ChatRoomDto(
            (string)room.Name, 
            (string)room.Description!
        );
    }
}
