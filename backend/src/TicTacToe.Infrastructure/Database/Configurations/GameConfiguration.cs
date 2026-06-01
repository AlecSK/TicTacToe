using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TicTacToe.Domain.Entities;

namespace TicTacToe.Infrastructure.Database.Configurations;

public class GameConfiguration : IEntityTypeConfiguration<Game>
{
    public void Configure(EntityTypeBuilder<Game> builder)
    {
        builder.ToTable("games");

        builder.HasKey(g => g.Id);
        builder.Property(g => g.Id).HasColumnName("id");
        builder.Property(g => g.PlayerId).HasColumnName("player_id");
        builder.Property(g => g.BoardState).HasColumnName("board_state").HasColumnType("char(9)").IsRequired();
        builder.Property(g => g.Status).HasColumnName("status").HasColumnType("smallint");
        builder.Property(g => g.PlayerStarts).HasColumnName("player_starts");
        builder.Property(g => g.StartedAt).HasColumnName("started_at");
        builder.Property(g => g.FinishedAt).HasColumnName("finished_at");

        builder.HasOne(g => g.Player)
            .WithMany(p => p.Games)
            .HasForeignKey(g => g.PlayerId);
    }
}
