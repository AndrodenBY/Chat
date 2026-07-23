using Chat.Domain.ValueObjects;

namespace Chat.Application.DTOs;

public record UserUpdateDto(Username Username, Email Email);
