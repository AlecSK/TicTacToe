using TicTacToe.Application.Services;

namespace TicTacToe.Infrastructure.Services;

public class GameEngine : IGameEngine
{
    private static readonly int[][] WinPatterns =
    [
        [0, 1, 2], [3, 4, 5], [6, 7, 8],
        [0, 3, 6], [1, 4, 7], [2, 5, 8],
        [0, 4, 8], [2, 4, 6]
    ];

    public char? CheckWinner(string boardState)
    {
        foreach (var pattern in WinPatterns)
        {
            var a = boardState[pattern[0]];
            if (a != '-' && a == boardState[pattern[1]] && a == boardState[pattern[2]])
                return a;
        }
        return null;
    }

    public bool IsDraw(string boardState) =>
        CheckWinner(boardState) is null && boardState.All(c => c != '-');

    public bool IsValidMove(string boardState, int cellIndex) =>
        cellIndex >= 0 && cellIndex <= 8 && boardState[cellIndex] == '-';
}
