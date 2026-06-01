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
            GetNextTurn(game.Status, game.BoardState, game.PlayerStarts)
        );

    public static MoveResultDto ToMoveResultDto(Game game, char playerMark, char aiMark, int? aiMove) =>
        new(
            game.Id,
            game.BoardState,
            game.Status.ToString(),
            playerMark,
            aiMark,
            GetNextTurn(game.Status, game.BoardState, game.PlayerStarts),
            aiMove
        );

    private static string? GetNextTurn(GameStatus status, string boardState, bool playerStarts)
    {
        if (status != GameStatus.InProgress)
            return null;

        var xCount = boardState.Count(c => c == 'X');
        var oCount = boardState.Count(c => c == 'O');

        // Player=X, AI=O.
        // playerStarts=true:  turns are X,O,X,O... → Player's turn when xCount==oCount
        // playerStarts=false: turns are O,X,O,X... → Player's turn when oCount>xCount (AI moved one more)
        return playerStarts
            ? (xCount == oCount ? "Player" : "AI")
            : (oCount > xCount ? "Player" : "AI");
    }
}
