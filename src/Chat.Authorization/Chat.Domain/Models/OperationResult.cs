namespace Chat.Domain.Models;

public sealed record OperationResult(OperationStatus Status, string? ErrorMessage = null);
