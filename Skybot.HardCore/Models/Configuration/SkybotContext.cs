namespace Skybot.HardCore.Models.Configuration.Configuration
{
    using Microsoft.EntityFrameworkCore;

    using Skybot.HardCore.Models;

    public class SkybotContext : DbContext
    {
        public SkybotContext(DbContextOptions<SkybotContext> options) : base(options) { }

        public virtual DbSet<DiscordUser> DiscordUsers { get; set; } = null!;
        public virtual DbSet<DiscordChannel> DiscordChannels { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Skybot");

            modelBuilder.Entity<DiscordUser>(entity =>
            {
                entity.ToTable("discordUsers");
                entity.Property(e => e.UserId);
                entity.Property(e => e.DiscordUserDiscriminator).HasPrecision(5);
                entity.Property(e => e.DiscordUserId).HasPrecision(20);
            });

            modelBuilder.Entity<DiscordChannel>(entity =>
            {
                entity.ToTable("discordChannels");
                entity.Property(e => e.ChannelId);
                entity.Property(e => e.DiscordChannelId).HasPrecision(20);
            });
        }
    }
}
