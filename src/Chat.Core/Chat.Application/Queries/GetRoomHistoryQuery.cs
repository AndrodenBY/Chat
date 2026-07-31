using Chat.Application.DTOs;
using Chat.Domain.Common.Pagination;
using Chat.Domain.Common.Result;
using DispatchR.Abstractions.Send;

namespace Chat.Application.Queries;

public record GetRoomHistoryQuery(long Id, PaginationParameters Parameters)
    : IRequest<GetRoomHistoryQuery, ValueTask<Result<PaginatedList<MessageDto>>>>;
