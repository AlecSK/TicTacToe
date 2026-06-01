using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TicTacToe.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "players",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    nickname = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    last_seen = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_players", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "games",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    player_id = table.Column<Guid>(type: "uuid", nullable: false),
                    board_state = table.Column<string>(type: "char(9)", nullable: false),
                    status = table.Column<short>(type: "smallint", nullable: false),
                    player_starts = table.Column<bool>(type: "boolean", nullable: false),
                    started_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    finished_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_games", x => x.id);
                    table.ForeignKey(
                        name: "FK_games_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "game_moves",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    game_id = table.Column<Guid>(type: "uuid", nullable: false),
                    move_number = table.Column<short>(type: "smallint", nullable: false),
                    cell_index = table.Column<short>(type: "smallint", nullable: false),
                    is_ai_move = table.Column<bool>(type: "boolean", nullable: false),
                    made_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_game_moves", x => x.id);
                    table.ForeignKey(
                        name: "FK_game_moves_games_game_id",
                        column: x => x.game_id,
                        principalTable: "games",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_game_moves_game_id_move_number",
                table: "game_moves",
                columns: new[] { "game_id", "move_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_games_player_id",
                table: "games",
                column: "player_id");

            migrationBuilder.CreateIndex(
                name: "IX_players_nickname",
                table: "players",
                column: "nickname",
                unique: true);

            migrationBuilder.Sql(@"
                CREATE VIEW leaderboard AS
                SELECT p.id, p.nickname,
                    COUNT(g.id) AS total_games,
                    COUNT(g.id) FILTER (WHERE g.status = 1) AS wins,
                    COUNT(g.id) FILTER (WHERE g.status = 2) AS losses,
                    COUNT(g.id) FILTER (WHERE g.status = 3) AS draws,
                    ROUND(COUNT(g.id) FILTER (WHERE g.status = 1)::numeric / NULLIF(COUNT(g.id), 0) * 100, 1) AS win_rate
                FROM players p
                LEFT JOIN games g ON g.player_id = p.id AND g.status != 0
                GROUP BY p.id, p.nickname
                ORDER BY wins DESC;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW IF EXISTS leaderboard;");

            migrationBuilder.DropTable(
                name: "game_moves");

            migrationBuilder.DropTable(
                name: "games");

            migrationBuilder.DropTable(
                name: "players");
        }
    }
}
