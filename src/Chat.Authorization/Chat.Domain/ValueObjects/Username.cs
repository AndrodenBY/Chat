using System.Text.RegularExpressions;

namespace Chat.Domain.ValueObjects;

public sealed partial record Username
{
    public string Value { get; }
    
    [GeneratedRegex(@"^[a-zA-Z0-9_-]+$")]
    private static partial Regex UsernameRegex();
    
    private Username(string value) => Value = value;

    public static bool TryCreate(string? input, out Username? username, out string? error)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            username = null;
            error = "Username cannot be empty";
            return false;
        }

        var trimmedUsername = input.Trim();

        if (trimmedUsername.Length is < 3 or > 25)
        {
            username = null;
            error = "Username must be between 3 and 25 characters";
            return false;
        }
        
        if (!UsernameRegex().IsMatch(trimmedUsername))
        {
            username = null;
            error = "Username can only contain letters, numbers, underscores, and hyphens.";
            return false;
        }

        username = new Username(trimmedUsername);
        error = null;
        return true;
    }
}
