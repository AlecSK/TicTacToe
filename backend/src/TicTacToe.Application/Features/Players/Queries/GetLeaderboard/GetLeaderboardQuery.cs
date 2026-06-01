using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Players.Queries.GetLeaderboard;

public record GetLeaderboardQuery(int Page, int PageSize) : IRequest<IReadOnlyList<LeaderboardEntryDto>>;
