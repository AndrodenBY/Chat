using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Room;

public record LeaveRoomCommand(string ConnectionId)
    : IRequest<LeaveRoomCommand, ValueTask<Result<bool>>>;
