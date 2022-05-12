// --------------------------------------------------------------------------------------------------------------------
// Filename : InfoModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 30.04.2022
// Last Modified On : 02.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules
{

    using Discord;
    using Discord.Commands;

    using Skybot.HardCore.Models;
    using Skybot.HardCore.Models.Configuration.Configuration;

    using System.Reflection;

    // Modules must be public and inherit from an IModuleBase
    public class InfoModule : ModuleBase<SocketCommandContext>
    {
        private readonly SkybotContext _databaseContext;

        public InfoModule(SkybotContext databaseContext) => _databaseContext = databaseContext;

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

        [Command("importUsers")]
        public async Task ImportUsersAsync()
        {
            if (Context.User.Id != 137352005818646529)
            {
                await ReplyAsync("User not authorized to run this command.");

                return;
            }

            await ReplyAsync($"Starting import of {Context.Guild.MemberCount} Guild members.");

            var users = Context.Guild.Users.Select(socketGuildUser => new DiscordUser
            {
                UserId = Guid.NewGuid(),
                IsBlocked = false,
                DiscordUserDiscriminator = socketGuildUser.DiscriminatorValue,
                DiscordUserDisplayName = socketGuildUser.DisplayName,
                DiscordUserId = socketGuildUser.Id
            }).ToList();

            await ReplyAsync("Filtering list to only import new users.");

            var filteredUsers = users.ExceptBy(_databaseContext.DiscordUsers.Select(u => u.UserId), u => u.UserId).ToList();

            await ReplyAsync($"Created {filteredUsers.Count} Discord user objects. Saving to database.");

            _databaseContext.DiscordUsers.AddRange(filteredUsers);
            await _databaseContext.SaveChangesAsync(CancellationToken.None);

            await ReplyAsync($"Imported {filteredUsers.Count} Guild members as Discord users. Total count of users is {_databaseContext.DiscordUsers.Count()}.");
        }

        [Command("importChannels")]
        public async Task ImportChannelsAsync()
        {
            if (Context.User.Id != 137352005818646529)
            {
                await ReplyAsync("User not authorized to run this command.");

                return;
            }

            var discordChannels = Context.Guild.Channels.Where(c => (c.GetChannelType() == ChannelType.Text) | (c.GetChannelType() == ChannelType.PublicThread)).ToList();

            await ReplyAsync($"Starting import of {discordChannels.Count} channels/threads.");

            var channels = discordChannels.Select(socketGuildChannel => new DiscordChannel
            {
                ChannelId = Guid.NewGuid(),
                DiscordChannelId = socketGuildChannel.Id,
                DiscordChannelName = socketGuildChannel.Name
            }).ToList();

            await ReplyAsync("Filtering list to only import new channels.");

            var filteredChannels = channels.ExceptBy(_databaseContext.DiscordChannels.Select(u => u.ChannelId), u => u.ChannelId).ToList();

            await ReplyAsync($"Created {filteredChannels.Count} Discord channel objects. Saving to database.");

            _databaseContext.DiscordChannels.AddRange(filteredChannels);
            await _databaseContext.SaveChangesAsync(CancellationToken.None);

            await ReplyAsync($"Imported {filteredChannels.Count} Guild channels as Discord channels. Total count of channels is {_databaseContext.DiscordChannels.Count()}.");
        }
    }
}
