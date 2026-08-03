using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Rooms;

public record DeleteRoomCommand(long Id) : IRequest<DeleteRoomCommand,  ValueTask<Result<bool>>>;
