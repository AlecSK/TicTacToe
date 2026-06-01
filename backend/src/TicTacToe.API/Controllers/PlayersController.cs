using TicTacToe.Application.Features.Players.Commands.LoginPlayer;
using TicTacToe.Application.Features.Players.Queries.GetPlayerStats;
using TicTacToe.Application.Features.Games.Queries.GetPlayerGames;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/players")]
public class PlayersController(IMediator mediator) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new LoginPlayerCommand(request.Nickname), ct);
        return Ok(result);
    }

    [HttpGet("{nickname}")]
    public async Task<IActionResult> GetStats(string nickname, CancellationToken ct)
    {
        var result = await mediator.Send(new GetPlayerStatsQuery(nickname), ct);
        return Ok(result);
    }

    [HttpGet("{nickname}/games")]
    public async Task<IActionResult> GetGames(
        string nickname,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetPlayerGamesQuery(nickname, page, pageSize), ct);
        return Ok(result);
    }
}

public record LoginRequest(string Nickname);
