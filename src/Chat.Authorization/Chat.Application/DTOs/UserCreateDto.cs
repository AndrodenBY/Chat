using Chat.Domain.ValueObjects;

namespace Chat.Application.DTOs;

public record UserCreateDto(Username Username, Email Email, string Password);
