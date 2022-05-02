// --------------------------------------------------------------------------------------------------------------------
// Filename : DatabaseContext.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.05.2022 00:40
// Last Modified On : 02.05.2022 17:49
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Database;

using Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

public class DatabaseContext : DbContext
{
    private readonly IConfiguration _configuration;

    public DatabaseContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public DatabaseContext(DbContextOptions<DatabaseContext> options)
        : base(options) { }

    public virtual DbSet<DiscordUser> DiscordUsers { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
            optionsBuilder.UseNpgsql(_configuration.GetValue<string>("Data:PostgresConnectionString"));
    }

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
    }
}
