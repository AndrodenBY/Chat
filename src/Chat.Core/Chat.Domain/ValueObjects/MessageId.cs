using Chat.Domain.Common;
using Chat.Domain.Common.Result;

namespace Chat.Domain.ValueObjects;

public readonly record struct MessageId
{
    public long Value { get; }
    
    private MessageId(long value) => Value = value;
    
    public static MessageId From(long value) => new(value);

    public static Result<MessageId> Create(long value)
    {
        if (value <= 0)
        {
            return Error.Validation("MessageId.MustBePositive", "Value must be greater than 0");
        }

        return new MessageId(value);
    }
}
