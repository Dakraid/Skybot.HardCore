// --------------------------------------------------------------------------------------------------------------------
// Filename : DatabaseContext.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.05.2022 00:40
// Last Modified On : 02.05.2022 22:03
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Database
{
    using Entities;

    using Microsoft.EntityFrameworkCore;

    public class DatabaseContext : DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }

        public virtual DbSet<DiscordUser> DiscordUsers { get; set; } = null!;
        public virtual DbSet<DiscordChannel> DiscordChannels { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<DiscordUser>(entity =>
            {
                entity.ToTable("discordUsers");

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("ID");

                entity.Property(e => e.UserDiscriminator).HasPrecision(5);
                entity.Property(e => e.UserId).HasPrecision(20);
            });

            modelBuilder.Entity<DiscordChannel>(entity =>
            {
                entity.ToTable("discordChannels");

                entity.Property(e => e.Id)
                      .ValueGeneratedNever()
                      .HasColumnName("ID");

                entity.Property(e => e.ChannelId).HasPrecision(20);
            });
        }
    }
}
