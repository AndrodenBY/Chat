namespace Chat.Domain.ValueObjects;

public readonly record struct UserConnection(string UserId, string Username, string RoomName);
