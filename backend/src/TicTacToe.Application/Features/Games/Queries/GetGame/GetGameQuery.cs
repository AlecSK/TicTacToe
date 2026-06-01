using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Games.Queries.GetGame;

public record GetGameQuery(Guid GameId) : IRequest<GameStateDto>;
