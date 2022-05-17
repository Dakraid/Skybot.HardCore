// --------------------------------------------------------------------------------------------------------------------
// Filename : SkybotContext.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 14.05.2022
// Last Modified On : 14.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Models.Configuration
{
    using Microsoft.EntityFrameworkCore;

    public class SkybotContext : DbContext
    {
        public SkybotContext(DbContextOptions<SkybotContext> options) : base(options) { }

        public virtual DbSet<DiscordUser> DiscordUsers { get; set; } = null!;
        public virtual DbSet<DiscordChannel> DiscordChannels { get; set; } = null!;
        public virtual DbSet<DiscordRole> DiscordRoles { get; set; } = null!;
        public virtual DbSet<BotPermission> BotPermissions { get; set; } = null!;
        public virtual DbSet<Factoid> Factoids { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("Skybot");

            modelBuilder.Entity<DiscordUser>(entity =>
            {
                entity.ToTable("discordUsers");
            });

            modelBuilder.Entity<DiscordChannel>(entity =>
            {
                entity.ToTable("discordChannels");
            });

            modelBuilder.Entity<DiscordRole>(entity =>
            {
                entity.ToTable("discordRoles");
            });

            modelBuilder.Entity<BotPermission>(entity =>
            {
                entity.ToTable("botPermissions");
            });

            modelBuilder.Entity<Factoid>(entity =>
            {
                entity.ToTable("factoids");
            });
        }
    }
}
