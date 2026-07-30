using Chat.Domain.Entities;

namespace Chat.Domain.Contracts;

public interface IChatClient
{
    Task ReceiveMessage(string username, Message message);
    Task UserJoined(string username, string roomName);
    Task UserLeft(string username, string roomName);
}
