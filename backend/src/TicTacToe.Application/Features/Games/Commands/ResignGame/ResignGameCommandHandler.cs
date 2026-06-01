using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Exceptions;
using TicTacToe.Application.Repositories;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Enums;

namespace TicTacToe.Application.Features.Games.Commands.ResignGame;

public class ResignGameCommandHandler(
    IPlayerRepository playerRepository,
    IGameRepository gameRepository)
    : IRequestHandler<ResignGameCommand, GameStateDto>
{
    private const char PlayerMark = 'X';
    private const char AiMark = 'O';

    public async Task<GameStateDto> Handle(ResignGameCommand request, CancellationToken ct)
    {
        var player = await playerRepository.GetByNicknameAsync(request.Nickname, ct)
            ?? throw new NotFoundException($"Player '{request.Nickname}' not found.");

        var game = await gameRepository.GetByIdAsync(request.GameId, ct)
            ?? throw new NotFoundException($"Game '{request.GameId}' not found.");

        if (game.PlayerId != player.Id)
            throw new BusinessException("This game does not belong to you.");

        if (game.Status != GameStatus.InProgress)
            throw new BusinessException("Game is already finished.");

        game.Status = GameStatus.AiWon;
        game.FinishedAt = DateTime.UtcNow;
        await gameRepository.UpdateAsync(game, ct);

        return GameStateHelper.ToGameStateDto(game, PlayerMark, AiMark);
    }
}
