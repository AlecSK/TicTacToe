using TicTacToe.Application.Features.Players.Queries.GetLeaderboard;

namespace TicTacToe.API.Controllers;

[ApiController]
[Route("api/leaderboard")]
public class LeaderboardController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetLeaderboard(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken ct = default)
    {
        var result = await mediator.Send(new GetLeaderboardQuery(page, pageSize), ct);
        return Ok(result);
    }
}
