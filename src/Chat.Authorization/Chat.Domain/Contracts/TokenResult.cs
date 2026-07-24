using Chat.Domain.ValueObjects;

namespace Chat.Domain.Contracts;

public sealed record TokenResult(
    AccessToken AccessToken,
    RefreshToken RefreshToken,
    DateTimeOffset AccessTokenExpiresAt,
    DateTimeOffset RefreshTokenExpiresAt
)
{
    public bool IsAccessTokenExpired(DateTimeOffset utcNow) => utcNow >= AccessTokenExpiresAt;
    public bool IsRefreshTokenExpired(DateTimeOffset utcNow) => utcNow >= RefreshTokenExpiresAt;
}
