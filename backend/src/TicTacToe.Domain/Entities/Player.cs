namespace TicTacToe.Domain.Entities;

public class Player
{
    public Guid Id { get; set; }
    public required string Nickname { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime LastSeen { get; set; }
    public ICollection<Game> Games { get; set; } = [];
}
