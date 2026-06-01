namespace TicTacToe.Application.DTOs;

public record LeaderboardEntryDto(
    Guid Id,
    string Nickname,
    long TotalGames,
    long Wins,
    long Losses,
    long Draws,
    decimal? WinRate
);
