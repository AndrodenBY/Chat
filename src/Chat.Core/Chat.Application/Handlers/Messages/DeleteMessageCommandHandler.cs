using Chat.Application.Commands.Messages;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Messages;

public class DeleteMessageCommandHandler(IRepository<Message, MessageId> messageRepository)
    : IRequestHandler<DeleteMessageCommand, ValueTask<Result<bool>>>
{
    public async ValueTask<Result<bool>> Handle(DeleteMessageCommand request, CancellationToken cancellationToken)
    {
        var messageId = MessageId.From(request.Id);
        var message = await messageRepository.GetById(messageId, cancellationToken);
        if (message is null)
        {
            return true;
        }
        
        return await messageRepository.Remove(message, cancellationToken);
    }
}
