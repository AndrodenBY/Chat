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

    public static Message Create(MessageId id, RoomId roomId, string senderId, MessageContent content, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(senderId))
        {
            // return "SenderId is required";
        }

        return new Message(id, roomId, senderId, content, createdAt);
    }
}
