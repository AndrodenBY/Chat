using Chat.Domain.ValueObjects;

namespace Chat.Application.DTOs;

public record UserUpdateDto(string Username, string Email);
