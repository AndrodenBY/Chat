namespace Chat.Domain.ValueObjects;

public sealed record RefreshToken
{
    public string  Value { get; }
    
    private RefreshToken(string value) => Value = value;

    public static bool TryCreate(string input, out RefreshToken? refreshToken, out string? error)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            refreshToken = null;
            error = "Refresh Token cannot be empty";
            return false;
        }

        var trimmedToken = input.Trim();

        if (trimmedToken.Length < 15)
        {
            refreshToken = null;
            error = "Refresh Token is too short";
            return false;
        }

        refreshToken = new RefreshToken(trimmedToken);
        error = null;
        return true;
    }
}
