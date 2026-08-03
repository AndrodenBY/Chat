using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Messages;

public record UpdateMessageCommand(long Id, string Content) 
    : IRequest<UpdateMessageCommand, ValueTask<Result<MessageDto>>>;
