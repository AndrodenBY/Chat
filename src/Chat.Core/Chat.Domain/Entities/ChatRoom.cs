using Chat.Domain.Common.Result;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Entities;

public class ChatRoom
{
    public RoomId Id { get; private set; }
    public RoomName Name { get; private set; }
    public RoomDescription? Description { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public DateTimeOffset UpdatedAt { get; private set; }
    
    public ICollection<Message> Messages { get; private set; } = [];
    
    private ChatRoom(RoomId id, RoomName name, RoomDescription? description, DateTimeOffset createdAt, DateTimeOffset updatedAt)
    {
        Id = id;
        Name = name;
        Description = description;
        CreatedAt = createdAt;
        UpdatedAt = updatedAt;
    }

    public static Result<ChatRoom> Create(
        RoomId id,
        string name,
        string? description = null,
        DateTimeOffset? createdAt = null)
    {
        var roomName = RoomName.Create(name);

        if (roomName.IsFailure)
        {
            return roomName.PrimaryError;
        }

        RoomDescription? roomDescription = null;

        if (description is not null)
        {
            var descriptionResult = RoomDescription.Create(description);

            if (descriptionResult.IsFailure)
            {
                return descriptionResult.PrimaryError;
            }

            roomDescription = descriptionResult.Value;
        }

        var now = createdAt ?? DateTimeOffset.UtcNow;

        return new ChatRoom(
            id,
            roomName.Value,
            roomDescription,
            now,
            now
        );
    }

    public Result<bool> UpdateDetails(string? name = null, string? description = null)
    {
        var hasChanges = false;

        if (name is not null)
        {
            var nameResult = RoomName.Create(name);
            if (nameResult.IsFailure)
            {
                return nameResult.PrimaryError;
            }

            if (nameResult.Value != Name)
            {
                Name = nameResult.Value;
                hasChanges = true;
            }
        }
        
        if (description is not null)
        {
            var descriptionResult = RoomDescription.Create(description);
            if (descriptionResult.IsFailure)
            {
                return descriptionResult.PrimaryError;
            }
            
            if (descriptionResult.Value != Description)
            {
                Description = descriptionResult.Value;
                hasChanges = true;
            }
        }
        
        return hasChanges;
    }
    
    public void Touch(DateTimeOffset? updatedAt = null)
    {
        UpdatedAt = updatedAt ?? DateTimeOffset.UtcNow;
    }
}
