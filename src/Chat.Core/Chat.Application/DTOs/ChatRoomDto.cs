namespace Chat.Application.DTOs;

public record ChatRoomDto(string Id, string Name, string? Description, DateTimeOffset CreatedAt, DateTimeOffset UpdatedAt);
