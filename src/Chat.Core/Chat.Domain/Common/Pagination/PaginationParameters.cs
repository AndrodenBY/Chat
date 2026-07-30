namespace Chat.Domain.Common.Pagination;

public readonly record struct PaginationParameters(int PageNumber = PaginationConstants.DefaultPageNumber, int PageSize = PaginationConstants.DefaultPageSize);
