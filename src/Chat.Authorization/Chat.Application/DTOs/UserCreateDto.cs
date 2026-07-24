using Chat.Domain.ValueObjects;

namespace Chat.Application.DTOs;

public record UserCreateDto(string Username, string Email, string Password);
