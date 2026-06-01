using MediatR;
using TicTacToe.Application.DTOs;
using TicTacToe.Application.Repositories;
using TicTacToe.Domain.Entities;

namespace TicTacToe.Application.Features.Players.Commands.LoginPlayer;

public class LoginPlayerCommandHandler(IPlayerRepository playerRepository)
    : IRequestHandler<LoginPlayerCommand, PlayerDto>
{
    public async Task<PlayerDto> Handle(LoginPlayerCommand request, CancellationToken ct)
    {
        var player = await playerRepository.GetByNicknameAsync(request.Nickname, ct);

        if (player is null)
        {
            player = new Player
            {
                Id = Guid.NewGuid(),
                Nickname = request.Nickname,
                CreatedAt = DateTime.UtcNow,
                LastSeen = DateTime.UtcNow
            };
            await playerRepository.AddAsync(player, ct);
        }
        else
        {
            player.LastSeen = DateTime.UtcNow;
            await playerRepository.UpdateAsync(player, ct);
        }

        return new PlayerDto(player.Id, player.Nickname, player.CreatedAt, player.LastSeen);
    }
}
