using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Rooms;

public record CreateRoomCommand(string Name, string? Description)
    : IRequest<CreateRoomCommand, ValueTask<Result<ChatRoomDto>>>;
