using Chat.Domain.ValueObjects;

namespace Chat.Api.Contracts;

public record LoginParameters(string Username, string Password);
