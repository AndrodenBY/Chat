namespace Chat.Domain.ValueObjects;

public readonly record struct RoomId
{
    public long Value { get; }
    
    private RoomId(long value) => Value = value;

    public static RoomId Create(long value)
    {
        if (value <= 0)
        {
            //return "RoomId must be a positive value";
        }

        return new RoomId(value);
    }
}
