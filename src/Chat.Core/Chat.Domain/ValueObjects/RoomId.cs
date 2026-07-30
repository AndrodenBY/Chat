using Chat.Domain.Common;

namespace Chat.Domain.ValueObjects;

public readonly record struct RoomId
{
    public long Value { get; }
    
    private RoomId(long value) => Value = value;

    public static Result<RoomId> Create(long value)
    {
        if (value <= 0)
        {
            return  Error.Validation("RoomId.MustBePositive", "Value must be greater than 0");
        }

        return new RoomId(value);
    }
}
