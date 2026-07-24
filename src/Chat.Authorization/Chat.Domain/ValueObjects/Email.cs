using System.Text.RegularExpressions;

namespace Chat.Domain.ValueObjects;

public sealed partial record Email
{
    public string Value { get; set; }
    
    [GeneratedRegex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
    private static partial Regex EmailRegex();
    
    private Email(string value) => Value = value;

    public static bool TryCreate(string? input, out Email? email, out string? error)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            email = null;
            error = "Email cannot be empty";
            return false;
        }
        
        var trimmedEmail = input.Trim();

        if (!EmailRegex().IsMatch(trimmedEmail))
        {
            email = null;
            error = "Email format is invalid";
            return false;
        }

        email = new Email(trimmedEmail);
        error = null;
        return true;
    }
}
