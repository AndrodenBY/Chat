using Chat.Application.Commands.Messages;
using Chat.Application.Contracts;
using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Messages;

public class SendMessageCommandHandler(
    IConnectionTracker connectionTracker,
    IRepository<Message, MessageId> messageRepository,
    ISnowflakeIdGenerator snowflakeIdGenerator,
    IChatClient chatClient) 
    : IRequestHandler<SendMessageCommand, ValueTask<Result<MessageDto>>>
{
    public async ValueTask<Result<MessageDto>> Handle(SendMessageCommand request, CancellationToken cancellationToken)
    {
        var connectionResult = (await connectionTracker.GetConnection(request.ConnectionId, cancellationToken))
            .OnError(Error.NotFound("ChatConnection.NotFound", "User connection was not found"));
        
        var contentResult = MessageContent.Create(request.Content);
        if (contentResult.IsFailure)
        {
            return contentResult.PrimaryError;
        }

        var messageId = MessageId.From(snowflakeIdGenerator.NextId());
        
        var connection = connectionResult.Value;
        var roomId = RoomId.From(connection.RoomId);
        
        var message = Message.Create(messageId, roomId, connection.UserId, contentResult);
        if (message.IsFailure)
        {
            return message.PrimaryError;
        }
        
        var messageInstance = await messageRepository.Add(message, cancellationToken);
        
        await chatClient.ReceiveMessage(connection.Username, messageInstance);

        return new MessageDto(
            (string)messageInstance.Content, 
            messageInstance.CreatedAt
        );
    }
}
