using Chat.Application.Commands.Messages;
using Chat.Application.DTOs;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Messages;

public class UpdateMessageCommandHandler(IRepository<Message, MessageId> messageRepository)
    : IRequestHandler<UpdateMessageCommand, ValueTask<Result<MessageDto>>>
{
    public async ValueTask<Result<MessageDto>> Handle(UpdateMessageCommand request, CancellationToken cancellationToken)
    {
        var messageId = MessageId.From(request.Id);
        var messageResult = (await messageRepository.GetById(messageId, cancellationToken))
            .OnError(Error.Validation("Message.NotFound", "Message was not found"));

        if (messageResult.IsFailure)
        {
            return messageResult.PrimaryError;
        }
        
        var message = messageResult.Value;
        var updateResult = message.UpdateDetails(request.Content);
        if (updateResult.IsFailure)
        {
            return updateResult.PrimaryError;
        }

        if (updateResult)
        {
            var repositoryUpdate = await messageRepository.Update(message, cancellationToken);
            return new MessageDto(
                (string)repositoryUpdate.Content, 
                repositoryUpdate.CreatedAt
            );    
        }

        return new MessageDto(
            (string)message.Content,
            message.CreatedAt
        );
    }
}
