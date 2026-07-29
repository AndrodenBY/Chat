using Chat.Domain.ValueObjects;

namespace Chat.Domain.Entities;

public class ChatRoom
{
    public RoomId Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }

    private ChatRoom(RoomId id, string name, string description, DateTimeOffset createdAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
    }

    public static ChatRoom Create(RoomId id, string name, string description, DateTimeOffset createdAt)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            // return "Chat room name cannot be empty";
        }
        
        return new ChatRoom(id, name, description, createdAt);
    }
}
