using TicTacToe.Application.Features.Games.Commands.MakeMove;
using TicTacToe.Application.Features.Games.Commands.ResignGame;
using TicTacToe.Application.Features.Games.Commands.StartGame;
using TicTacToe.Application.Features.Games.Queries.GetGame;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/games")]
public class GamesController(IMediator mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> StartGame([FromBody] StartGameRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new StartGameCommand(request.Nickname, request.PlayerStarts), ct);
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetGame(Guid id, CancellationToken ct)
    {
        var result = await mediator.Send(new GetGameQuery(id), ct);
        return Ok(result);
    }

    [HttpPost("{id:guid}/moves")]
    public async Task<IActionResult> MakeMove(Guid id, [FromBody] MakeMoveRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new MakeMoveCommand(id, request.Nickname, request.CellIndex), ct);
        return Ok(result);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Resign(Guid id, [FromBody] ResignRequest request, CancellationToken ct)
    {
        var result = await mediator.Send(new ResignGameCommand(id, request.Nickname), ct);
        return Ok(result);
    }
}

public record StartGameRequest(string Nickname, bool PlayerStarts);
public record MakeMoveRequest(string Nickname, int CellIndex);
public record ResignRequest(string Nickname);
