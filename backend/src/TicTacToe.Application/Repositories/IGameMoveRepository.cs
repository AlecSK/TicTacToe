using TicTacToe.Domain.Entities;

namespace TicTacToe.Application.Repositories;

public interface IGameMoveRepository
{
    Task AddAsync(GameMove move, CancellationToken ct = default);
}
