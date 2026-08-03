using Chat.Domain.Common.Result;

namespace Chat.Domain.ValueObjects;

public class RoomDescription
{
    public const int MaxLength = 500;
    
    public string Value { get; }
    
    private RoomDescription(string value) =>  Value = value;

    public static Result<RoomDescription> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("ChatRoom.DescriptionRequired", "Room description cannot be empty");
        }
        
        var trimmedValue = value.Trim();

        if (trimmedValue.Length > MaxLength)
        {
            return Error.Validation("ChatRoom.DescriptionTooLong", $"Room description cannot exceed {MaxLength} characters");
        }
        
        return new RoomDescription(trimmedValue);
    }
    
    public static explicit operator string(RoomDescription description) => description.Value;
}
