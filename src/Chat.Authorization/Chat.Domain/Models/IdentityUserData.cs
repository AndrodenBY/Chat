using Chat.Domain.ValueObjects;

namespace Chat.Domain.Models;

public sealed record IdentityUserData(
    ExternalId ExternalId,
    Username Username,
    Email Email,
    bool Enabled);
