using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToe.Domain.Entities;

namespace TicTacToe.Infrastructure.Database.Configurations;

public class GameMoveConfiguration : IEntityTypeConfiguration<GameMove>
{
    public void Configure(EntityTypeBuilder<GameMove> builder)
    {
        builder.ToTable("game_moves");

        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).HasColumnName("id").UseIdentityAlwaysColumn();
        builder.Property(m => m.GameId).HasColumnName("game_id");
        builder.Property(m => m.MoveNumber).HasColumnName("move_number").HasColumnType("smallint");
        builder.Property(m => m.CellIndex).HasColumnName("cell_index").HasColumnType("smallint");
        builder.Property(m => m.IsAiMove).HasColumnName("is_ai_move");
        builder.Property(m => m.MadeAt).HasColumnName("made_at");

        builder.HasOne(m => m.Game)
            .WithMany(g => g.Moves)
            .HasForeignKey(m => m.GameId);

        builder.HasIndex(m => new { m.GameId, m.MoveNumber }).IsUnique();
    }
}
