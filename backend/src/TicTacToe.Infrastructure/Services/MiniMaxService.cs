using TicTacToe.Application.Services;

namespace TicTacToe.Infrastructure.Services;

public class MiniMaxService(IGameEngine gameEngine) : IMiniMaxService
{
    public int GetBestMove(string boardState, char aiMark, char playerMark)
    {
        var board = boardState.ToCharArray();
        int bestScore = int.MinValue;
        int bestIndex = -1;

        for (int i = 0; i < 9; i++)
        {
            if (board[i] != '-')
                continue;

            board[i] = aiMark;
            int score = MiniMax(board, false, aiMark, playerMark);
            board[i] = '-';

            if (score > bestScore)
            {
                bestScore = score;
                bestIndex = i;
            }
        }

        return bestIndex;
    }

    private int MiniMax(char[] board, bool isMaximizing, char aiMark, char playerMark)
    {
        var state = new string(board);
        var winner = gameEngine.CheckWinner(state);

        if (winner == aiMark) return 10;
        if (winner == playerMark) return -10;
        if (gameEngine.IsDraw(state)) return 0;

        if (isMaximizing)
        {
            int best = int.MinValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != '-')
                    continue;
                board[i] = aiMark;
                best = Math.Max(best, MiniMax(board, false, aiMark, playerMark));
                board[i] = '-';
            }
            return best;
        }
        else
        {
            int best = int.MaxValue;
            for (int i = 0; i < 9; i++)
            {
                if (board[i] != '-')
                    continue;
                board[i] = playerMark;
                best = Math.Min(best, MiniMax(board, true, aiMark, playerMark));
                board[i] = '-';
            }
            return best;
        }
    }
}
