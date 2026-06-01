namespace TicTacToe.Domain.Entities;

public class GameMove
{
    public long Id { get; set; }
    public Guid GameId { get; set; }
    public short MoveNumber { get; set; }
    public short CellIndex { get; set; }
    public bool IsAiMove { get; set; }
    public DateTime MadeAt { get; set; }
    public Game Game { get; set; } = null!;
}
