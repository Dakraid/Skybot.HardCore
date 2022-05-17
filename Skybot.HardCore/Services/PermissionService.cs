// --------------------------------------------------------------------------------------------------------------------
// Filename : HelperService.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 14.05.2022
// Last Modified On : 14.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Services
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using Enums;

    using Interfaces;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Models;
    using Models.Configuration;

    using System.Text;

    public class PermissionService
    {
        private readonly IConfiguration _configuration;
        private readonly SkybotContext _database;
        private readonly DiscordSocketClient _discord;

        private readonly ILogService _log;
        private readonly IServiceProvider _services;

        public PermissionService(IServiceProvider services, IConfiguration configuration, SkybotContext database)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _log = services.GetRequiredService<ILogService>();
            _services = services;
            _configuration = configuration;
            _database = database;
        }

        private async Task<string> AddSpecialPermission(string commandName, SpecialPermission specialPermission)
        {
            await _database.BotPermissions.AddAsync(new BotPermission
            {
                Command = commandName,
                PermissionId = Guid.NewGuid(),
                SpecialPermission = specialPermission
            });
            await _database.SaveChangesAsync();

            return "Special permission added to command.";
        }

        private async Task<string> AddUserPermission(string commandName, ulong id)
        {
            var user = await _database.DiscordUsers.FindAsync(id);

            if (user == null)
            {
                return "User not found, permission can't be set. If you believe this to be an error, please inform the developer.";
            }

            await _database.BotPermissions.AddAsync(new BotPermission
            {
                Command = commandName,
                PermissionId = Guid.NewGuid(),
                DiscordUsers = new List<DiscordUser> { user }
            });
            await _database.SaveChangesAsync();

            return "Granted user permission";
        }

        private async Task<string> AddRolePermission(string commandName, ulong id)
        {
            var role = await _database.DiscordRoles.FindAsync(id);

            if (role == null)
            {
                return "Role not found, permission can't be set. If you believe this to be an error, please inform the developer.";
            }

            await _database.BotPermissions.AddAsync(new BotPermission
            {
                Command = commandName,
                PermissionId = Guid.NewGuid(),
                DiscordRoles = new List<DiscordRole> { role }
            });
            await _database.SaveChangesAsync();

            return "Granted role permission";
        }

        public async Task<string> AddPermission(SocketCommandContext context, string commandName, PermissionType type, ulong id, SpecialPermission? specialPermission = null)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                await _log.LogAsync(LogSeverity.Warning, nameof(PermissionService), "No commandName given, permission modification can't be executed.");

                return "No commandName given, permission modification can't be executed.";
            }

            var commandPermissions = _database.BotPermissions.FirstOrDefault(p => p.Command == commandName);

            if (commandPermissions == null)
            {
                await _log.LogAsync(LogSeverity.Error, nameof(PermissionService), "Command has no permission entry in database. Adding entry.");

                if (specialPermission != null)
                {
                    return await AddSpecialPermission(commandName, specialPermission.Value);
                }

                switch (type)
                {
                    case PermissionType.User:
                        return await AddUserPermission(commandName, id);

                    case PermissionType.Role:
                        return await AddRolePermission(commandName, id);
                }
            }

            await _log.LogAsync(LogSeverity.Error, nameof(PermissionService), "Reached an unexpected execution end within SetPermission.");

            return "You are not supposed to be able to get here, please inform the developer";
        }

        public async Task<string> ListPermissions(SocketCommandContext context, string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                await _log.LogAsync(LogSeverity.Warning, nameof(PermissionService), "No commandName given, permission check can't be executed.");

                return "No commandName given, permission modification can't be executed.";
            }

            var commandPermissions = _database.BotPermissions.FirstOrDefault(p => p.Command == commandName);

            if (commandPermissions == null)
            {
                await _log.LogAsync(LogSeverity.Info, nameof(PermissionService), "Command has no permission entry in database. Adding entry with default (Owner only), please adjust as required.");

                var newEntry = _database.BotPermissions.Add(new BotPermission
                {
                    Command = commandName,
                    PermissionId = Guid.NewGuid(),
                    SpecialPermission = SpecialPermission.Owner
                });

                await _database.SaveChangesAsync();

                commandPermissions = newEntry.Entity;
            }

            var stringBuilder = new StringBuilder();
            stringBuilder.Append($"Command '{commandName}' has following permissions:\n");

            if (commandPermissions.DiscordRoles.Any())
            {
                stringBuilder.AppendLine($"  Roles: {string.Join(", ", commandPermissions.DiscordRoles)}");
            }

            if (commandPermissions.DiscordUsers.Any())
            {
                stringBuilder.AppendLine($"  Users: {string.Join(", ", commandPermissions.DiscordUsers)}");
            }

            stringBuilder.AppendLine($"  Special: {commandPermissions.SpecialPermission}");

            return stringBuilder.ToString();
        }

        public async Task<bool> VerifyPermission(SocketCommandContext context, string commandName)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                await _log.LogAsync(LogSeverity.Error, nameof(PermissionService), "No commandName given, permission check can't be executed. This should not happen, please notify the developer.");

                return false;
            }

            var commandPermissions = _database.BotPermissions.FirstOrDefault(p => p.Command == commandName);

            if (commandPermissions == null || commandPermissions.SpecialPermission == SpecialPermission.Owner)
            {
                var dataUser = await _database.DiscordUsers.FindAsync(context.User.Id);
                if (dataUser != null)
                {
                    await _database.Entry(dataUser).ReloadAsync();
                }

                var ownerCheck = (await _database.DiscordUsers.FindAsync(context.User.Id))?.IsOwner;
                if (ownerCheck.HasValue && ownerCheck.Value)
                {
                    return true;
                }

                if (commandPermissions != null)
                {
                    return false;
                }

                await _log.LogAsync(LogSeverity.Info, nameof(PermissionService), "Command has no permission entry in database. Adding entry with default (Owner only), please adjust as required.");

                _database.BotPermissions.Add(new BotPermission
                {
                    Command = commandName,
                    PermissionId = Guid.NewGuid(),
                    SpecialPermission = commandName == "help" ? SpecialPermission.Anyone : SpecialPermission.Owner
                });

                await _database.SaveChangesAsync();

                return commandName == "help";
            }

            if (commandPermissions.DiscordUsers.Any())
            {
                if (commandPermissions.DiscordUsers.Exists(u => u.DiscordUserId == context.User.Id))
                {
                    return true;
                }
            }

            var userRoles = (context.User as SocketGuildUser)?.Roles;

            if (userRoles != null)
            {
                switch (commandPermissions.SpecialPermission)
                {
                    case SpecialPermission.Disabled:
                        return false;

                    case SpecialPermission.AnyRole:
                        return userRoles.Any();

                    case SpecialPermission.Anyone:
                        return true;

                    case SpecialPermission.None:
                    case SpecialPermission.Owner:
                    default:
                        break;
                }

                var commandRoles = commandPermissions.DiscordRoles;

                var match = userRoles.IntersectBy(commandRoles.Select(r => r.DiscordRoleId), r => r.Id).ToList();

                if (match.Any())
                {
                    return true;
                }
            }

            return false;
        }
    }
}
