using System.Diagnostics.CodeAnalysis;
using Chat.Application.Commands.Rooms;
using Chat.Domain.Common.Result;
using Chat.Domain.Contracts;
using Chat.Domain.Entities;
using Chat.Domain.ValueObjects;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Handlers.Rooms;

public class LeaveRoomCommandHandler(IConnectionTracker connectionTracker)
    : IRequestHandler<LeaveRoomCommand, ValueTask<Result<bool>>>
{
    public async ValueTask<Result<bool>> Handle(LeaveRoomCommand request, CancellationToken cancellationToken)
    {
        await connectionTracker.RemoveConnection(request.ConnectionId, cancellationToken);

        return true;
    }
}
