namespace Chat.Domain.ValueObjects;

public readonly record struct MessageContent
{
    public const int MaxLength = 2000;
    public string Value { get; }
    
    private MessageContent(string value) => Value = value;

    public static MessageContent Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            // return "MessageContent cannot be empty";
        }

        if (value.Length > MaxLength)
        {
            // return "Message content cannot exceed MaxLength";
        }

        return new MessageContent(value);
    }
}
