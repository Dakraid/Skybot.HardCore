// --------------------------------------------------------------------------------------------------------------------
// Filename : InfoModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 30.04.2022 17:00
// Last Modified On : 02.05.2022 17:49
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules;

using System.Reflection;

using Database;

using Discord;
using Discord.Commands;

using Entities;

// Modules must be public and inherit from an IModuleBase
public class InfoModule : ModuleBase<SocketCommandContext>
{
    [Command("about")]
    public Task AboutAsync()
    {
        var version = Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "Unknown";
        var color = (uint)Random.Shared.Next(0, 16777215);
        var icon = Context.Client.CurrentUser.GetAvatarUrl() ?? "https://cdn.discordapp.com/embed/avatars/0.png";
        var name = Context.Client.CurrentUser.Username ?? "Skybot.HardCore";

        var embed = new EmbedBuilder
        {
            Title = "Running using Skybot.HardCore",
            Description = $"Currently used version is {version}.",
            Color = new Color(color),
            Author = new EmbedAuthorBuilder
            {
                IconUrl = icon,
                Name = name
            },
            Footer = new EmbedFooterBuilder
            {
                IconUrl = "https://cdn.discordapp.com/avatars/137352005818646529/248d65f033a78bc2d5df832f9dbcaf04.png?size=4096",
                Text = "Developed by Netrve"
            },
            ThumbnailUrl = icon
        };
        embed.WithCurrentTimestamp();

        return ReplyAsync(embed: embed.Build());
    }

    [Command("pingDb")]
    public Task PingDbAsync()
    {
        using var databaseContext = new DatabaseContext();

        var users = databaseContext.DiscordUsers.ToList();

        return ReplyAsync($"I know {users.Count} Discord users.");
    }

    [Command("importUsers")]
    public async Task ImportUsersAsync()
    {
        await using var databaseContext = new DatabaseContext();
        await ReplyAsync($"Starting import of {Context.Guild.MemberCount} Guild members.");

        var users = Context.Guild.Users.Select(socketGuildUser => new DiscordUser
        {
            Id = Guid.NewGuid(),
            IsBlocked = false,
            UserDiscriminator = socketGuildUser.DiscriminatorValue,
            UserDisplayName = socketGuildUser.DisplayName,
            UserId = socketGuildUser.Id
        }).ToList();
        await ReplyAsync($"Created {users.Count} Discord user objects. Saving to database.");

        databaseContext.DiscordUsers.AddRange(users);
        await databaseContext.SaveChangesAsync(CancellationToken.None);

        await ReplyAsync($"Imported {databaseContext.DiscordUsers.Count()} Guild members as Discord users.");
    }
}
