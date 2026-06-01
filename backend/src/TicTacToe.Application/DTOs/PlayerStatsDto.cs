namespace TicTacToe.Application.DTOs;

public record PlayerStatsDto(
    Guid Id,
    string Nickname,
    DateTime CreatedAt,
    DateTime LastSeen,
    long TotalGames,
    long Wins,
    long Losses,
    long Draws,
    decimal? WinRate
);
