namespace Chat.Domain.ValueObjects;

public record UserConnection(string ConnectionId, string UserId, string Username, long RoomId);
