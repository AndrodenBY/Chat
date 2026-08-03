using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Rooms;

public record UpdateRoomCommand(long Id, string Name, string? Description)
    : IRequest<UpdateRoomCommand, Result<ChatRoomDto>>;
