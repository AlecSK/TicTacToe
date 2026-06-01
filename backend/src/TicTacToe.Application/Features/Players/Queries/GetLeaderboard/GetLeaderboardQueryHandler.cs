using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Repositories;

namespace TicTacToe.Application.Features.Players.Queries.GetLeaderboard;

public class GetLeaderboardQueryHandler(ILeaderboardRepository leaderboardRepository)
    : IRequestHandler<GetLeaderboardQuery, IReadOnlyList<LeaderboardEntryDto>>
{
    public Task<IReadOnlyList<LeaderboardEntryDto>> Handle(GetLeaderboardQuery request, CancellationToken ct) =>
        leaderboardRepository.GetAsync(request.Page, request.PageSize, ct);
}
