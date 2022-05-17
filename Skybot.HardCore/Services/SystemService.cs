// --------------------------------------------------------------------------------------------------------------------
// Filename : SystemService.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 16.05.2022
// Last Modified On : 17.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Services
{
    using Discord;
    using Discord.WebSocket;

    using Interfaces;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Models;
    using Models.Configuration;

    public class SystemService
    {
        private readonly IConfiguration _configuration;
        private readonly SkybotContext _database;
        private readonly DiscordSocketClient _discord;

        private readonly ILogService _log;
        private readonly IServiceProvider _services;

        public SystemService(IServiceProvider services, IConfiguration configuration, SkybotContext database)
        {
            _discord       = services.GetRequiredService<DiscordSocketClient>();
            _log           = services.GetRequiredService<ILogService>();
            _services      = services;
            _configuration = configuration;
            _database      = database;
        }

        public async Task InitializeAsync()
        {
            await ScanUsersAsync();
            await ScanChannelsAsync();
            await ScanRolesAsync();
        }

        public async Task ScanUsersAsync()
        {
            var context = _discord.Guilds.FirstOrDefault();

            if (context == null)
            {
                await _log.LogAsync(LogSeverity.Error, nameof(SystemService), "No Guild found.");

                return;
            }

            await _log.LogAsync(LogSeverity.Info, nameof(SystemService), $"Scanning {context.MemberCount} Guild members.");

            var oldCount = _database.DiscordUsers.Count();

            var users = context.Users.Select(socketGuildUser => new DiscordUser
            {
                IsBlocked = false, DiscordUserDiscriminator = socketGuildUser.DiscriminatorValue, DiscordUserDisplayName = socketGuildUser.DisplayName, DiscordUserId = socketGuildUser.Id
            }).ToList();

            var newUsers = users.ExceptBy(_database.DiscordUsers.Select(e => e.DiscordUserId), x => x.DiscordUserId).ToList();

            if (newUsers.Any())
            {
                await _database.DiscordUsers.AddRangeAsync(newUsers);
                await _database.SaveChangesAsync(CancellationToken.None);
            }

            try
            {
                _database.DiscordUsers.UpdateRange(users);
                await _database.SaveChangesAsync(CancellationToken.None);
            }
            catch
            {
                // we ignore the error as there is nothing to update
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), "No updates were made.");
            }

            if (oldCount == _database.DiscordUsers.Count())
            {
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), "No additions were made.");
            }
            else
            {
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), $"Old total count was {oldCount}, new total count of users is {_database.DiscordUsers.Count()}.");
            }
        }

        public async Task ScanChannelsAsync()
        {
            var context = _discord.Guilds.FirstOrDefault();

            if (context == null)
            {
                await _log.LogAsync(LogSeverity.Error, nameof(SystemService), "No Guild found.");

                return;
            }

            var discordChannels = context.Channels.Where(c => (c.GetChannelType() == ChannelType.Text) | (c.GetChannelType() == ChannelType.PublicThread)).ToList();

            await _log.LogAsync(LogSeverity.Info, nameof(SystemService), $"Scanning {discordChannels.Count} channels/threads.");

            var oldCount = _database.DiscordChannels.Count();

            var channels = discordChannels.Select(socketGuildChannel => new DiscordChannel
            {
                DiscordChannelId = socketGuildChannel.Id, DiscordChannelName = socketGuildChannel.Name
            }).ToList();

            var newChannels = channels.ExceptBy(_database.DiscordChannels.Select(e => e.DiscordChannelId), x => x.DiscordChannelId).ToList();

            if (newChannels.Any())
            {
                await _database.DiscordChannels.AddRangeAsync(newChannels);
                await _database.SaveChangesAsync(CancellationToken.None);
            }

            try
            {
                _database.DiscordChannels.UpdateRange(channels);
                await _database.SaveChangesAsync(CancellationToken.None);
            }
            catch
            {
                // we ignore the error as there is nothing to update
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), "No updates were made.");
            }

            if (oldCount == _database.DiscordChannels.Count())
            {
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), "No additions were made.");
            }
            else
            {
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), $"Old total count was {oldCount}, new total count of channels is {_database.DiscordChannels.Count()}.");
            }
        }

        public async Task ScanRolesAsync()
        {
            var context = _discord.Guilds.FirstOrDefault();

            if (context == null)
            {
                await _log.LogAsync(LogSeverity.Error, nameof(SystemService), "No Guild found.");

                return;
            }

            var discordRoles = context.Roles.ToList();

            await _log.LogAsync(LogSeverity.Info, nameof(SystemService), $"Scanning {discordRoles.Count} roles.");

            var oldCount = _database.DiscordRoles.Count();

            var roles = discordRoles.Select(socketRole => new DiscordRole
            {
                DiscordRoleId = socketRole.Id, DiscordRoleName = socketRole.Name
            }).ToList();

            var newRoles = roles.ExceptBy(_database.DiscordRoles.Select(e => e.DiscordRoleId), x => x.DiscordRoleId).ToList();

            if (newRoles.Any())
            {
                await _database.DiscordRoles.AddRangeAsync(newRoles);
                await _database.SaveChangesAsync(CancellationToken.None);
            }

            try
            {
                _database.DiscordRoles.UpdateRange(roles);
                await _database.SaveChangesAsync(CancellationToken.None);
            }
            catch
            {
                // we ignore the error as there is nothing to update
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), "No updates were made.");
            }

            if (oldCount == _database.DiscordRoles.Count())
            {
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), "No additions were made.");
            }
            else
            {
                await _log.LogAsync(LogSeverity.Info, nameof(SystemService), $"Old total count was {oldCount}, new total count of roles is {_database.DiscordRoles.Count()}.");
            }
        }
    }
}
