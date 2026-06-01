using TicTacToe.Application.Repositories;
using TicTacToe.Domain.Entities;
using TicTacToe.Infrastructure.Database;

namespace TicTacToe.Infrastructure.Repositories;

public class GameMoveRepository(TicTacToeDbContext db) : IGameMoveRepository
{
    public async Task AddAsync(GameMove move, CancellationToken ct = default)
    {
        db.GameMoves.Add(move);
        await db.SaveChangesAsync(ct);
    }
}
