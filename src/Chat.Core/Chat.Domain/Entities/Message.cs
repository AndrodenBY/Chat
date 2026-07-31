using Chat.Domain.Common.Result;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Entities;

public class Message
{
    public MessageId Id { get; private set; }
    public RoomId RoomId { get; private set; }
    public string SenderId { get; private set; }
    public MessageContent Content { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private Message(MessageId id, RoomId roomId, string senderId, MessageContent content, DateTimeOffset createdAt)
    {
        Id = id;
        RoomId = roomId;
        SenderId = senderId;
        Content = content;
        CreatedAt = createdAt;
    }

    public static Result<Message> Create(MessageId id, RoomId roomId, string senderId, MessageContent content, DateTimeOffset? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(senderId))
        {
            return Error.Validation("Create.InvalidSenderId", "SenderId cannot be empty");
        }
        
        var now = createdAt ?? DateTimeOffset.UtcNow;

        return new Message(id, roomId, senderId, content, now);
    }

    public Result<bool> UpdateDetails(string? content = null)
    {
        var contentResult = MessageContent.Create(content);

        if (contentResult.IsFailure)
        {
            return contentResult.PrimaryError;
        }

        if (contentResult.Value != Content)
        {
            Content = contentResult.Value;
            return true;
        }

        return false;
    }
}
