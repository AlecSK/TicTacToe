using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Exceptions;
using TicTacToe.Application.Repositories;
using TicTacToe.Application.Services;

namespace TicTacToe.Application.Features.Games.Queries.GetGame;

public class GetGameQueryHandler(IGameRepository gameRepository)
    : IRequestHandler<GetGameQuery, GameStateDto>
{
    private const char PlayerMark = 'X';
    private const char AiMark = 'O';

    public async Task<GameStateDto> Handle(GetGameQuery request, CancellationToken ct)
    {
        var game = await gameRepository.GetByIdAsync(request.GameId, ct)
            ?? throw new NotFoundException($"Game '{request.GameId}' not found.");

        return GameStateHelper.ToGameStateDto(game, PlayerMark, AiMark);
    }
}
