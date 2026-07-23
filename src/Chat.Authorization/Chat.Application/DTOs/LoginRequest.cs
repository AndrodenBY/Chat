using Chat.Domain.ValueObjects;

namespace Chat.Application.DTOs;

public record LoginRequest(Username Username, string Password);
