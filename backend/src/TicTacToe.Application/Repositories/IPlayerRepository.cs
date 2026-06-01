using TicTacToe.Domain.Entities;

namespace TicTacToe.Application.Repositories;

public interface IPlayerRepository
{
    Task<Player?> GetByNicknameAsync(string nickname, CancellationToken ct = default);
    Task<Player?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<Player> AddAsync(Player player, CancellationToken ct = default);
    Task UpdateAsync(Player player, CancellationToken ct = default);
}
