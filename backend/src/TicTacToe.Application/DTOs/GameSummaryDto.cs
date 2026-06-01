namespace TicTacToe.Application.DTOs;

public record GameSummaryDto(
    Guid GameId,
    string Status,
    bool PlayerStarts,
    DateTime StartedAt,
    DateTime? FinishedAt,
    string BoardState
);
