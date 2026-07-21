namespace Chat.Application.Contracts;

public class AuthorizationResponse
{
    public required string AccessToken { get; init; }
    public required string RefreshToken { get; init; }
    public required DateTimeOffset AccessTokenExpiresIn { get; init; }
    public required DateTimeOffset RefreshTokenExpiresIn { get; init; }
}
