using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Games.Commands.MakeMove;

public record MakeMoveCommand(Guid GameId, string Nickname, int CellIndex) : IRequest<MoveResultDto>;
