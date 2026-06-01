using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Games.Commands.StartGame;

public record StartGameCommand(string Nickname, bool PlayerStarts) : IRequest<GameStateDto>;
