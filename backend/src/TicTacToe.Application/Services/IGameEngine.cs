namespace TicTacToe.Application.Services;

public interface IGameEngine
{
    char? CheckWinner(string boardState);
    bool IsDraw(string boardState);
    bool IsValidMove(string boardState, int cellIndex);
}
