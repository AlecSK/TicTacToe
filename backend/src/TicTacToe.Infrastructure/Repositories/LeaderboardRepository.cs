using Microsoft.EntityFrameworkCore;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Repositories;
using TicTacToe.Infrastructure.Database;
using TicTacToe.Infrastructure.Database.Views;

namespace TicTacToe.Infrastructure.Repositories;

public class LeaderboardRepository(TicTacToeDbContext db) : ILeaderboardRepository
{
    public async Task<LeaderboardEntryDto?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default)
    {
        var entry = await db.Set<LeaderboardEntryView>()
            .FirstOrDefaultAsync(e => e.Id == playerId, ct);

        return entry is null ? null : Map(entry);
    }

    public async Task<IReadOnlyList<LeaderboardEntryDto>> GetAsync(int page, int pageSize, CancellationToken ct = default)
    {
        var entries = await db.Set<LeaderboardEntryView>()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return entries.Select(Map).ToList();
    }

    private static LeaderboardEntryDto Map(LeaderboardEntryView e) =>
        new(e.Id, e.Nickname, e.TotalGames, e.Wins, e.Losses, e.Draws, e.WinRate);
}
