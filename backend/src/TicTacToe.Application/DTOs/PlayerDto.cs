namespace TicTacToe.Application.DTOs;

public record PlayerDto(
    Guid Id,
    string Nickname,
    DateTime CreatedAt,
    DateTime LastSeen
);
