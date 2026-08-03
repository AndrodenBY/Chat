using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Messages;

public record DeleteMessageCommand(long Id) : IRequest<DeleteMessageCommand, ValueTask<Result<bool>>>;
