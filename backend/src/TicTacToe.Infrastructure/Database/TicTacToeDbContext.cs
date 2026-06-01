using Microsoft.EntityFrameworkCore;
using TicTacToe.Domain.Entities;
using TicTacToe.Infrastructure.Database.Views;

namespace TicTacToe.Infrastructure.Database;

public class TicTacToeDbContext(DbContextOptions<TicTacToeDbContext> options) : DbContext(options)
{
    public DbSet<Player> Players => Set<Player>();
    public DbSet<Game> Games => Set<Game>();
    public DbSet<GameMove> GameMoves => Set<GameMove>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicTacToeDbContext).Assembly);

        modelBuilder.Entity<LeaderboardEntryView>(e =>
        {
            e.HasNoKey();
            e.ToView("leaderboard");
            e.Property(x => x.Id).HasColumnName("id");
            e.Property(x => x.Nickname).HasColumnName("nickname");
            e.Property(x => x.TotalGames).HasColumnName("total_games");
            e.Property(x => x.Wins).HasColumnName("wins");
            e.Property(x => x.Losses).HasColumnName("losses");
            e.Property(x => x.Draws).HasColumnName("draws");
            e.Property(x => x.WinRate).HasColumnName("win_rate");
        });
    }
}
