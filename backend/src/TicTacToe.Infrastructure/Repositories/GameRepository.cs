using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.Repositories;
using TicTacToe.Domain.Entities;
using TicTacToe.Infrastructure.Database;

namespace TicTacToe.Infrastructure.Repositories;

public class GameRepository(TicTacToeDbContext db) : IGameRepository
{
    public async Task<Game?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Games.Include(g => g.Moves).FirstOrDefaultAsync(g => g.Id == id, ct);

    public async Task<IReadOnlyList<Game>> GetByPlayerIdAsync(
        Guid playerId, int page, int pageSize, CancellationToken ct = default) =>
        await db.Games
            .Where(g => g.PlayerId == playerId)
            .OrderByDescending(g => g.StartedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<Game> AddAsync(Game game, CancellationToken ct = default)
    {
        db.Games.Add(game);
        await db.SaveChangesAsync(ct);
        return game;
    }

    public async Task UpdateAsync(Game game, CancellationToken ct = default)
    {
        db.Games.Update(game);
        await db.SaveChangesAsync(ct);
    }
}
