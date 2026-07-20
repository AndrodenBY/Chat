namespace Chat.Application.Contracts;

public record TokenResult(string AccessToken, string RefreshToken, int ExpiresIn);
