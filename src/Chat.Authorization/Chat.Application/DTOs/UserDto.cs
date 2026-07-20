namespace Chat.Application.DTOs;

public record UserDto(string ExternalId, string Username, string Email, bool Enabled);
