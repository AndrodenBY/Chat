namespace Chat.Application.DTOs;

public record UserUpdateDto(string ExternalId, string NewUsername,string NewEmail);
