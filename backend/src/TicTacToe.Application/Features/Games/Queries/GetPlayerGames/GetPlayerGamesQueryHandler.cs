using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Exceptions;
using TicTacToe.Application.Repositories;

namespace TicTacToe.Application.Features.Games.Queries.GetPlayerGames;

public class GetPlayerGamesQueryHandler(
    IPlayerRepository playerRepository,
    IGameRepository gameRepository)
    : IRequestHandler<GetPlayerGamesQuery, IReadOnlyList<GameSummaryDto>>
{
    public async Task<IReadOnlyList<GameSummaryDto>> Handle(GetPlayerGamesQuery request, CancellationToken ct)
    {
        var player = await playerRepository.GetByNicknameAsync(request.Nickname, ct)
            ?? throw new NotFoundException($"Player '{request.Nickname}' not found.");

        var games = await gameRepository.GetByPlayerIdAsync(player.Id, request.Page, request.PageSize, ct);

        return games
            .Select(g => new GameSummaryDto(
                g.Id,
                g.Status.ToString(),
                g.PlayerStarts,
                g.StartedAt,
                g.FinishedAt,
                g.BoardState))
            .ToList();
    }
}
