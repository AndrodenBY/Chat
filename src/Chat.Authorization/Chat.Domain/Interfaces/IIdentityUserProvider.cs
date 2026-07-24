using Chat.Domain.Models;
using Chat.Domain.ValueObjects;

namespace Chat.Domain.Interfaces;

public interface IIdentityUserProvider
{
    Task<IdentityUserData?> Get(ExternalId externalId, CancellationToken cancellationToken);

    Task<ExternalId> Create(Username username, Email email, string password, CancellationToken cancellationToken);

    Task<OperationResult> Update(ExternalId externalId, Username username, Email email, CancellationToken cancellationToken);

    Task<OperationResult> Delete(ExternalId externalId, CancellationToken cancellationToken);
}
