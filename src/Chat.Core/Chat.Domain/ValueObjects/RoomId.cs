using Chat.Domain.Common;
using Chat.Domain.Common.Result;

namespace Chat.Domain.ValueObjects;

public readonly record struct RoomId
{
    public long Value { get; }

    private RoomId(long value) => Value = value;
    
    public static RoomId From(long value) => new(value);
    
    public static Result<RoomId> Create(long value)
    {
        if (value <= 0)
        {
            return Error.Validation("RoomId.MustBePositive", "Room ID must be greater than 0.");
        }

        return new RoomId(value);
    }

    public static implicit operator long(RoomId id) => id.Value;
    public override string ToString() => Value.ToString();
}
