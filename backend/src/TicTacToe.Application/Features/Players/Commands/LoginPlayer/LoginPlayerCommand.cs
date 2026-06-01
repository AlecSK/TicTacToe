using MediatR;
using TicTacToe.Application.DTOs;

namespace TicTacToe.Application.Features.Players.Commands.LoginPlayer;

public record LoginPlayerCommand(string Nickname) : IRequest<PlayerDto>;
