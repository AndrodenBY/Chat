using Chat.Domain.Common.Result;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Room;

public record JoinRoomCommand(string ConnectionId, UserConnection Connection) 
    : IRequest<JoinRoomCommand, ValueTask<Result<string>>>;
