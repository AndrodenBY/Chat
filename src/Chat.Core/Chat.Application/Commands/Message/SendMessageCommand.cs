using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Commands.Message;

public record SendMessageCommand(string ConnectionId, string Content) 
    : IRequest<SendMessageCommand, ValueTask<Result<MessageDto>>>;
