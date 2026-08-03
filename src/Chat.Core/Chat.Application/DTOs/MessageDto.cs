namespace Chat.Application.DTOs;

public record MessageDto(string Content, DateTimeOffset CreatedAt);
