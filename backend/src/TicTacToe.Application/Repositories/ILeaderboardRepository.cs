using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Repositories;

public interface ILeaderboardRepository
{
    Task<LeaderboardEntryDto?> GetByPlayerIdAsync(Guid playerId, CancellationToken ct = default);
    Task<IReadOnlyList<LeaderboardEntryDto>> GetAsync(int page, int pageSize, CancellationToken ct = default);
}
