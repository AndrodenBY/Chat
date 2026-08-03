using Chat.Domain.Contracts.Specification;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Specifications;

public class MessagesByRoomSpec : BaseSpecification<Message>
{
    public MessagesByRoomSpec(RoomId roomId) : base(message => message.RoomId == roomId)
    {
        AddOrderByDescending(message => message.CreatedAt);
    }
}
