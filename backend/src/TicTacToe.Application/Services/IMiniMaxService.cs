namespace TicTacToe.Application.Services;

public interface IMiniMaxService
{
    int GetBestMove(string boardState, char aiMark, char playerMark);
}
