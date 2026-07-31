using Chat.Domain.Contracts.Specification;
using Chat.Domain.Entities;

namespace Chat.Domain.Specifications;

public class ChatRoomByNameSpec(string name)
    : BaseSpecification<ChatRoom>(room => room.Name.Value.Equals(name, StringComparison.CurrentCultureIgnoreCase));

