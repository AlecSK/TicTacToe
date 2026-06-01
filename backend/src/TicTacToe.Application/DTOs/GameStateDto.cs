namespace TicTacToe.Application.DTOs;

public record GameStateDto(
    Guid GameId,
    string BoardState,
    string Status,
    char PlayerMark,
    char AiMark,
    string? NextTurn
);
