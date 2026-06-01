using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Games.Queries.GetPlayerGames;

public record GetPlayerGamesQuery(string Nickname, int Page, int PageSize) : IRequest<IReadOnlyList<GameSummaryDto>>;
