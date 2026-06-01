using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace TicTacToe.Infrastructure.Database;

public class TicTacToeDbContextFactory : IDesignTimeDbContextFactory<TicTacToeDbContext>
{
    public TicTacToeDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<TicTacToeDbContext>()
            .UseNpgsql("Host=localhost;Port=5432;Database=tictactoe;Username=postgres;Password=postgres")
            .Options;
        return new TicTacToeDbContext(options);
    }
}
