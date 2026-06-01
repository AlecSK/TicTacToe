namespace TicTacToe.Application.DTOs;

public record MoveResultDto(
    Guid GameId,
    string BoardState,
    string Status,
    char PlayerMark,
    char AiMark,
    string? NextTurn,
    int? AiMove
);
