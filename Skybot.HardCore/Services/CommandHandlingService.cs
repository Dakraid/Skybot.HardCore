// --------------------------------------------------------------------------------------------------------------------
// Filename : CommandHandlingService.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 30.04.2022 17:00
// Last Modified On : 02.05.2022 22:01
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Services
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using System.Reflection;

    public class CommandHandlingService
    {
        private readonly CommandService _commands;
        private readonly IConfiguration _configuration;
        private readonly DiscordSocketClient _discord;
        private readonly LogService _log;
        private readonly IServiceProvider _services;

        public CommandHandlingService(IServiceProvider services, IConfiguration configuration)
        {
            _commands = services.GetRequiredService<CommandService>();
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _log = services.GetRequiredService<LogService>();
            _services = services;
            _configuration = configuration;

            _commands.CommandExecuted += CommandExecutedAsync;
            _discord.MessageReceived += MessageReceivedAsync;
        }

        public async Task InitializeAsync() => await _commands.AddModulesAsync(Assembly.GetEntryAssembly(), _services);

        private async Task MessageReceivedAsync(SocketMessage rawMessage)
        {
            if (rawMessage is not SocketUserMessage message)
            {
                return;
            }

            if (message.Source != MessageSource.User)
            {
                return;
            }

            var argPos = 0;

            if (!message.HasCharPrefix(_configuration.GetSection("Bot")["Trigger"][0], ref argPos) && !message.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                return;
            }

            await _log.LogAsync(new LogMessage(LogSeverity.Info, nameof(CommandHandlingService), $"Received command '{message}' from user '{message.Author.Username}'"));

            var context = new SocketCommandContext(_discord, message);
            await _commands.ExecuteAsync(context, argPos, _services);
        }

        private static async Task CommandExecutedAsync(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            // command is unspecified when there was a search failure (command not found); we don't care about these errors
            if (!command.IsSpecified)
            {
                return;
            }

            // the command was successful, we don't care about this result, unless we want to log that a command succeeded.
            if (result.IsSuccess)
            {
                return;
            }

            // the command failed, let's notify the user that something happened.
            await context.Channel.SendMessageAsync($"Error: {result}");
        }
    }
}
