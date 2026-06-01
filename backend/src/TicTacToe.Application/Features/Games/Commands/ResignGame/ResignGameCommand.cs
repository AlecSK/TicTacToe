using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Games.Commands.ResignGame;

public record ResignGameCommand(Guid GameId, string Nickname) : IRequest<GameStateDto>;
