using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Room;

public record DeleteRoomCommand(long Id) : IRequest<DeleteRoomCommand,  ValueTask<Result<bool>>>;
