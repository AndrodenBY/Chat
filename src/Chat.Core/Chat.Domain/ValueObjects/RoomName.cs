using Chat.Domain.Common.Result;

namespace Chat.Domain.ValueObjects;

public readonly record struct RoomName
{
    public const int MaxLength = 70;
    
    public string Value { get; }
    
    private RoomName(string value) =>  Value = value;

    public static Result<RoomName> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("ChatRoom.NameRequired", "Room name cannot be empty");
        }
        
        var trimmedValue = value.Trim();

        if (trimmedValue.Length > MaxLength)
        {
            return Error.Validation("ChatRoom.NameTooLong", $"Room name cannot exceed {MaxLength} characters");
        }
        
        return new RoomName(trimmedValue);
    }
    
    public static explicit operator string(RoomName name) => name.Value;
}
