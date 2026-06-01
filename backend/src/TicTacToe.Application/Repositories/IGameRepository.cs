using TicTacToe.Domain.Entities;

namespace TicTacToe.Application.Repositories;

public interface IGameRepository
{
    Task<Game?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<IReadOnlyList<Game>> GetByPlayerIdAsync(Guid playerId, int page, int pageSize, CancellationToken ct = default);
    Task<Game> AddAsync(Game game, CancellationToken ct = default);
    Task UpdateAsync(Game game, CancellationToken ct = default);
}
