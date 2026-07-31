using DispatchR.Abstractions.Send;
using Chat.Application.DTOs;
using Chat.Domain.Common.Result;

namespace Chat.Application.Commands;

public record SendMessageCommand(string ConnectionId, string Content) 
    : IRequest<SendMessageCommand, ValueTask<Result<MessageDto>>>;
