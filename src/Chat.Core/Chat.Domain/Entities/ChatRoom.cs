using Chat.Domain.Common;
using Chat.Domain.Common.Result;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Entities;

public class ChatRoom
{
    public RoomId Id { get; private set; }
    public string Name { get; private set; }
    public string? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public ICollection<Message> Messages { get; private set; } = [];
    
    private ChatRoom(RoomId id, string name, string? description, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Result<ChatRoom> Create(RoomId id, string name, string? description = null, DateTimeOffset? createdAt = null)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return Error.Validation("ChatRoom.NameRequired", "Name cannot be empty");
        }
        
        var now = createdAt ?? DateTimeOffset.UtcNow;
        
        return new ChatRoom(
            id, 
            name.Trim(), 
            description?.Trim(),
            now,
            now
        );
    }
    
    public void Touch(DateTimeOffset updatedAt)
    {
        UpdatedAt = updatedAt;
    }
}
