namespace Chat.Domain.ValueObjects;

public readonly record struct MessageId
{
    public long Value { get; }
    
    private MessageId(long value) => Value = value;

    public static MessageId Create(long value)
    {
        if (value <= 0)
        {
            //return "MessageId must be a positive value";
        }

        return new MessageId(value);
    }
}
