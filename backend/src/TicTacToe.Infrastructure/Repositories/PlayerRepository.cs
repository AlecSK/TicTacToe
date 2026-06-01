using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.Repositories;
using TicTacToe.Domain.Entities;
using TicTacToe.Infrastructure.Database;

namespace TicTacToe.Infrastructure.Repositories;

public class PlayerRepository(TicTacToeDbContext db) : IPlayerRepository
{
    public async Task<Player?> GetByNicknameAsync(string nickname, CancellationToken ct = default) =>
        await db.Players.FirstOrDefaultAsync(p => p.Nickname == nickname, ct);

    public async Task<Player?> GetByIdAsync(Guid id, CancellationToken ct = default) =>
        await db.Players.FindAsync([id], ct);

    public async Task<Player> AddAsync(Player player, CancellationToken ct = default)
    {
        db.Players.Add(player);
        await db.SaveChangesAsync(ct);
        return player;
    }

    public async Task UpdateAsync(Player player, CancellationToken ct = default)
    {
        db.Players.Update(player);
        await db.SaveChangesAsync(ct);
    }
}
