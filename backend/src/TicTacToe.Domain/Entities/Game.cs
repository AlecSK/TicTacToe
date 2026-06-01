using TicTacToe.Domain.Enums;

namespace TicTacToe.Domain.Entities;

public class Game
{
    public Guid Id { get; set; }
    public Guid PlayerId { get; set; }
    public required string BoardState { get; set; }
    public GameStatus Status { get; set; }
    public bool PlayerStarts { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishedAt { get; set; }
    public Player Player { get; set; } = null!;
    public ICollection<GameMove> Moves { get; set; } = [];
}
