// --------------------------------------------------------------------------------------------------------------------
// Filename : SystemModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 30.04.2022
// Last Modified On : 17.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules
{
    using Discord;
    using Discord.Commands;

    using Microsoft.Extensions.DependencyInjection;

    using Models.Configuration;

    using Services;

    using System.Reflection;
    using System.Text;

    // Modules must be public and inherit from an IModuleBase
    public class SystemModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commands;
        private readonly SkybotContext _databaseContext;
        private readonly PermissionService _permissions;
        private readonly SystemService _system;

        public SystemModule(IServiceProvider services, SkybotContext databaseContext, PermissionService permissions, SystemService system)
        {
            _commands = services.GetRequiredService<CommandService>();
            _databaseContext = databaseContext;
            _permissions = permissions;
            _system = system;
        }

        [Command("help")]
        [Summary("Provides help about the available commands.")]
        public async Task Help()
        {
            if (!await _permissions.VerifyPermission(Context, "help"))
            {
                await ReplyAsync("You have no permissions to access this command.");

                return;
            }

            var commands = _commands.Commands.ToList();
            var embedBuilder = new EmbedBuilder();

            foreach (var command in commands)
            {
                var parameters = new StringBuilder();

                foreach (var parameter in command.Parameters)
                {
                    var paramSummary = parameter.Summary ?? "No description available";
                    parameters.AppendLine($"- {parameter.Name}: {paramSummary}");
                }

                var embedFieldText = command.Summary ?? "No description available";

                if (parameters.Length > 0)
                {
                    embedFieldText += "\nParameters:\n" + parameters;
                }

                var group = !string.IsNullOrWhiteSpace(command.Module.Group) ? command.Module.Group + " " : "";
                embedBuilder.AddField(group + command.Name, embedFieldText);
            }

            await ReplyAsync("Here's a list of commands and their description: ", false, embedBuilder.Build());
        }

        [Command("about")]
        [Summary("Provides details about the bot.")]
        public async Task AboutAsync()
        {
            if (!await _permissions.VerifyPermission(Context, "about"))
            {
                await ReplyAsync("You have no permissions to access this command.");

                return;
            }

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

            await ReplyAsync(embed: embed.Build());
        }

        [Command("scanUsers")]
        [Summary("Refreshes the user table.")]
        public async Task ScanUsersAsync()
        {
            if (!await _permissions.VerifyPermission(Context, "importUsers"))
            {
                await ReplyAsync("You have no permissions to access this command.");

                return;
            }

            await ReplyAsync("Running scan, check log for details.");
            await _system.ScanUsersAsync();
        }

        [Command("scanChannels")]
        [Summary("Refreshes the channel table.")]
        public async Task ScanChannelsAsync()
        {
            if (!await _permissions.VerifyPermission(Context, "importChannels"))
            {
                await ReplyAsync("You have no permissions to access this command.");

                return;
            }

            await ReplyAsync("Running scan, check log for details.");
            await _system.ScanChannelsAsync();
        }

        [Command("scanRoles")]
        [Summary("Refreshes the role table.")]
        public async Task ScanRolesAsync()
        {
            if (!await _permissions.VerifyPermission(Context, "importRoles"))
            {
                await ReplyAsync("You have no permissions to access this command.");

                return;
            }

            await ReplyAsync("Running scan, check log for details.");
            await _system.ScanRolesAsync();
        }
    }
}
