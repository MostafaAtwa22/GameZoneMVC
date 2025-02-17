using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameZone.Repository.Configurations
{
    public class GameConfig : IEntityTypeConfiguration<Game>
    {
        public void Configure(EntityTypeBuilder<Game> builder)
        {
            builder.HasOne(g => g.category)
                .WithMany(c => c.Games)
                .HasForeignKey(g => g.CategoryId)
                .IsRequired(true);

            builder.HasMany(g => g.Devices)
            .WithMany(d => d.Games)
            .UsingEntity<GameDevice>(
                l => l.HasOne(x => x.Device)
                      .WithMany(d => d.GameDevices)
                      .HasForeignKey(x => x.DeviceId),

                r => r.HasOne(x => x.Game)
                      .WithMany(g => g.GameDevices)
                      .HasForeignKey(x => x.GameId),

                j =>
                {
                    j.HasKey(x => new { x.DeviceId, x.GameId });
                }
            );
        }
    }
}
