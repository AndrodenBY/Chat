using Chat.Domain.Common.Result;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands;

public record LeaveRoomCommand(string ConnectionId)
    : IRequest<LeaveRoomCommand, ValueTask<Result<bool>>>;
