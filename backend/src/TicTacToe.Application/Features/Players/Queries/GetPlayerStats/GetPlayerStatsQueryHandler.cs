using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Exceptions;
using TicTacToe.Application.Repositories;

namespace TicTacToe.Application.Features.Players.Queries.GetPlayerStats;

public class GetPlayerStatsQueryHandler(
    IPlayerRepository playerRepository,
    ILeaderboardRepository leaderboardRepository)
    : IRequestHandler<GetPlayerStatsQuery, PlayerStatsDto>
{
    public async Task<PlayerStatsDto> Handle(GetPlayerStatsQuery request, CancellationToken ct)
    {
        var player = await playerRepository.GetByNicknameAsync(request.Nickname, ct)
            ?? throw new NotFoundException($"Player '{request.Nickname}' not found.");

        var stats = await leaderboardRepository.GetByPlayerIdAsync(player.Id, ct);

        return new PlayerStatsDto(
            player.Id,
            player.Nickname,
            player.CreatedAt,
            player.LastSeen,
            stats?.TotalGames ?? 0,
            stats?.Wins ?? 0,
            stats?.Losses ?? 0,
            stats?.Draws ?? 0,
            stats?.WinRate
        );
    }
}
