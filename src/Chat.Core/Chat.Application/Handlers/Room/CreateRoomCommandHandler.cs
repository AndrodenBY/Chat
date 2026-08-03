using Chat.Application.Commands;
using Chat.Application.Commands.Room;
using Chat.Application.Contracts;
using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Room;

public class CreateRoomCommandHandler(
    IRepository<ChatRoom, RoomId> roomRepository,
    ISnowflakeIdGenerator idGenerator) 
    : IRequestHandler<CreateRoomCommand, ValueTask<Result<ChatRoomDto>>>
{
    public async ValueTask<Result<ChatRoomDto>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var roomId = RoomId.From(idGenerator.NextId());
        var roomResult = ChatRoom.Create(roomId, request.Name, request.Description);
        if (roomResult.IsFailure)
        {
            return roomResult.PrimaryError;
        }
        
        var roomInstance = await roomRepository.Add(roomResult, cancellationToken);
        
        return new ChatRoomDto(
            (string)roomInstance.Name, 
            (string)roomInstance.Description!
        );
    }
}
