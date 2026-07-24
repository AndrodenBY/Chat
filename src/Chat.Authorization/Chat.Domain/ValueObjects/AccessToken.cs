namespace Chat.Domain.ValueObjects;

public sealed record AccessToken
{
    public string  Value { get; }
    
    private AccessToken(string value) => Value = value;

    public static bool TryCreate(string? input, out AccessToken? accessToken, out string? error)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            accessToken = null;
            error = "Access Token cannot be empty";
            return false;
        }
        
        var trimmedToken = input.Trim();

        if (trimmedToken.Split('.').Length != 3)
        {
            accessToken = null;
            error = "Invalid Access Token format";
            return false;
        }

        accessToken = new AccessToken(trimmedToken);
        error = null;
        return true;
    }
}
