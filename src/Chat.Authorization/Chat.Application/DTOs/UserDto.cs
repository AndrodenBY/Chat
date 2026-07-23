using Chat.Domain.ValueObjects;

namespace Chat.Application.DTOs;

public record UserDto(ExternalId ExternalId, Username Username, Email Email, bool Enabled);
