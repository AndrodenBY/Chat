using Chat.Domain.Common;
using Chat.Domain.Common.Result;

namespace Chat.Domain.ValueObjects;

public readonly record struct MessageContent
{
    public const int MaxLength = 2000;
    public string Value { get; }
    
    private MessageContent(string value) => Value = value;

    public static Result<MessageContent> Create(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Error.Validation("Message.ContentRequired", "Message content cannot be empty");
        }

        if (value.Length > MaxLength)
        {
            return Error.Validation("Message.ContentTooLong", "Message content cannot be longer than 2000 characters");
        }

        return new MessageContent(value);
    }
    
    public static explicit operator string(MessageContent content) => content.Value;
}
