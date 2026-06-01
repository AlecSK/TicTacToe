using TicTacToe.Application.DTOs;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;

namespace TicTacToe.Application.Services;

public static class GameStateHelper
{
    public static GameStateDto ToGameStateDto(Game game, char playerMark, char aiMark) =>
        new(
            game.Id,
            game.BoardState,
            game.Status.ToString(),
            playerMark,
            aiMark,
            GetNextTurn(game.Status, game.BoardState)
        );

    public static MoveResultDto ToMoveResultDto(Game game, char playerMark, char aiMark, int? aiMove) =>
        new(
            game.Id,
            game.BoardState,
            game.Status.ToString(),
            playerMark,
            aiMark,
            GetNextTurn(game.Status, game.BoardState),
            aiMove
        );

    private static string? GetNextTurn(GameStatus status, string boardState)
    {
        if (status != GameStatus.InProgress)
            return null;

        var xCount = boardState.Count(c => c == 'X');
        var oCount = boardState.Count(c => c == 'O');
        return xCount == oCount ? "Player" : "AI";
    }
}
