using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Exceptions;
using TicTacToe.Application.Repositories;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;

namespace TicTacToe.Application.Features.Games.Commands.StartGame;

public class StartGameCommandHandler(
    IPlayerRepository playerRepository,
    IGameRepository gameRepository,
    IMiniMaxService miniMaxService)
    : IRequestHandler<StartGameCommand, GameStateDto>
{
    private const char PlayerMark = 'X';
    private const char AiMark = 'O';

    public async Task<GameStateDto> Handle(StartGameCommand request, CancellationToken ct)
    {
        var player = await playerRepository.GetByNicknameAsync(request.Nickname, ct)
            ?? throw new NotFoundException($"Player '{request.Nickname}' not found. Please login first.");

        var boardState = "---------";
        short moveNumber = 1;
        var moves = new List<GameMove>();

        if (!request.PlayerStarts)
        {
            var aiIndex = miniMaxService.GetBestMove(boardState, AiMark, PlayerMark);
            var board = boardState.ToCharArray();
            board[aiIndex] = AiMark;
            boardState = new string(board);

            moves.Add(new GameMove
            {
                Id = 0,
                GameId = Guid.Empty,
                MoveNumber = moveNumber,
                CellIndex = (short)aiIndex,
                IsAiMove = true,
                MadeAt = DateTime.UtcNow
            });
        }

        var game = new Game
        {
            Id = Guid.NewGuid(),
            PlayerId = player.Id,
            BoardState = boardState,
            Status = GameStatus.InProgress,
            PlayerStarts = request.PlayerStarts,
            StartedAt = DateTime.UtcNow
        };

        await gameRepository.AddAsync(game, ct);

        foreach (var move in moves)
        {
            move.GameId = game.Id;
        }

        if (moves.Count > 0)
        {
            game.Moves = moves;
            await gameRepository.UpdateAsync(game, ct);
        }

        return GameStateHelper.ToGameStateDto(game, PlayerMark, AiMark);
    }
}
