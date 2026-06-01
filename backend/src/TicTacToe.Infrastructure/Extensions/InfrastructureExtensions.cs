using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TicTacToe.Application.Repositories;
using TicTacToe.Application.Services;
using TicTacToe.Infrastructure.Database;
using TicTacToe.Infrastructure.Repositories;
using TicTacToe.Infrastructure.Services;

namespace TicTacToe.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<TicTacToeDbContext>(opts =>
            opts.UseNpgsql(connectionString));

        services.AddScoped<IPlayerRepository, PlayerRepository>();
        services.AddScoped<IGameRepository, GameRepository>();
        services.AddScoped<IGameMoveRepository, GameMoveRepository>();
        services.AddScoped<ILeaderboardRepository, LeaderboardRepository>();

        services.AddSingleton<IGameEngine, GameEngine>();
        services.AddSingleton<IMiniMaxService, MiniMaxService>();

        return services;
    }
}
