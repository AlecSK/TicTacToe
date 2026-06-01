using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Exceptions;
using TicTacToe.Application.Repositories;
using TicTacToe.Application.Services;
using TicTacToe.Domain.Entities;
using TicTacToe.Domain.Enums;

namespace TicTacToe.Application.Features.Games.Commands.MakeMove;

public class MakeMoveCommandHandler(
    IPlayerRepository playerRepository,
    IGameRepository gameRepository,
    IGameEngine gameEngine,
    IMiniMaxService miniMaxService)
    : IRequestHandler<MakeMoveCommand, MoveResultDto>
{
    private const char PlayerMark = 'X';
    private const char AiMark = 'O';

    public async Task<MoveResultDto> Handle(MakeMoveCommand request, CancellationToken ct)
    {
        var player = await playerRepository.GetByNicknameAsync(request.Nickname, ct)
            ?? throw new NotFoundException($"Player '{request.Nickname}' not found.");

        var game = await gameRepository.GetByIdAsync(request.GameId, ct)
            ?? throw new NotFoundException($"Game '{request.GameId}' not found.");

        if (game.PlayerId != player.Id)
            throw new BusinessException("This game does not belong to you.");

        if (game.Status != GameStatus.InProgress)
            throw new BusinessException("Game is already finished.");

        if (!gameEngine.IsValidMove(game.BoardState, request.CellIndex))
            throw new BusinessException($"Cell {request.CellIndex} is not available.");

        var board = game.BoardState.ToCharArray();
        var nextMoveNumber = (short)(game.Moves.Count + 1);

        // Player move
        board[request.CellIndex] = PlayerMark;
        game.BoardState = new string(board);
        game.Moves.Add(new GameMove
        {
            GameId = game.Id,
            MoveNumber = nextMoveNumber,
            CellIndex = (short)request.CellIndex,
            IsAiMove = false,
            MadeAt = DateTime.UtcNow
        });

        if (gameEngine.CheckWinner(game.BoardState) == PlayerMark)
        {
            game.Status = GameStatus.PlayerWon;
            game.FinishedAt = DateTime.UtcNow;
            await gameRepository.UpdateAsync(game, ct);
            return GameStateHelper.ToMoveResultDto(game, PlayerMark, AiMark, null);
        }

        if (gameEngine.IsDraw(game.BoardState))
        {
            game.Status = GameStatus.Draw;
            game.FinishedAt = DateTime.UtcNow;
            await gameRepository.UpdateAsync(game, ct);
            return GameStateHelper.ToMoveResultDto(game, PlayerMark, AiMark, null);
        }

        // AI move
        var aiIndex = miniMaxService.GetBestMove(game.BoardState, AiMark, PlayerMark);
        board[aiIndex] = AiMark;
        game.BoardState = new string(board);
        game.Moves.Add(new GameMove
        {
            GameId = game.Id,
            MoveNumber = (short)(nextMoveNumber + 1),
            CellIndex = (short)aiIndex,
            IsAiMove = true,
            MadeAt = DateTime.UtcNow
        });

        if (gameEngine.CheckWinner(game.BoardState) == AiMark)
        {
            game.Status = GameStatus.AiWon;
            game.FinishedAt = DateTime.UtcNow;
        }
        else if (gameEngine.IsDraw(game.BoardState))
        {
            game.Status = GameStatus.Draw;
            game.FinishedAt = DateTime.UtcNow;
        }

        await gameRepository.UpdateAsync(game, ct);
        return GameStateHelper.ToMoveResultDto(game, PlayerMark, AiMark, aiIndex);
    }
}
