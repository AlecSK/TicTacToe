namespace TicTacToe.Infrastructure.Database.Views;

internal class LeaderboardEntryView
{
    public Guid Id { get; set; }
    public string Nickname { get; set; } = "";
    public long TotalGames { get; set; }
    public long Wins { get; set; }
    public long Losses { get; set; }
    public long Draws { get; set; }
    public decimal? WinRate { get; set; }
}
