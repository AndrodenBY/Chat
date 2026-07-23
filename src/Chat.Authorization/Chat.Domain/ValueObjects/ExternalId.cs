namespace Chat.Domain.ValueObjects;

public sealed record ExternalId
{
    public string Value { get; }
    
    private ExternalId(string value) => Value = value;

    public static bool TryCreate(string? input, out ExternalId? externalId, out string? error)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            externalId = null;
            error = "ExternalId cannot be empty";
            return false;
        }
        
        var trimmedId = input.Trim();

        if (trimmedId.Length > 100)
        {
            externalId = null;
            error = "ExternalId cannot exceed 100 characters";
            return false;
        }
        
        externalId = new ExternalId(trimmedId);
        error = null;
        return true;
    }
}
