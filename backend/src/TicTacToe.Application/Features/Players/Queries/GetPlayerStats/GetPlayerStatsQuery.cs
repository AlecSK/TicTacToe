using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Players.Queries.GetPlayerStats;

public record GetPlayerStatsQuery(string Nickname) : IRequest<PlayerStatsDto>;
