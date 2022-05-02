// --------------------------------------------------------------------------------------------------------------------
// Filename : Program.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 26.12.2021 21:46
// Last Modified On : 02.05.2022 22:01
// Copyrights : Copyright (c) Kristian Schlikow 2021-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore
{
    using Database;

    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Services;

    public class Program
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceProvider _services;

        private readonly DiscordSocketConfig _socketConfig = new()
        {
            LogLevel = LogSeverity.Info, MessageCacheSize = 100, GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers, AlwaysDownloadUsers = true
        };

        private Program()
        {
            _configuration = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json")
                             .Build();

            _services = new ServiceCollection()
                        .AddSingleton(_configuration)
                        .AddSingleton(_socketConfig)
                        .AddSingleton<LogService>()
                        .AddSingleton<DiscordSocketClient>()
                        .AddSingleton<CommandService>()
                        .AddSingleton<CommandHandlingService>()
                        .AddSingleton<HttpClient>()
                        .AddSingleton<PictureService>()
                        .AddDbContext<DatabaseContext>(options =>
                        {
                            options.UseNpgsql(_configuration.GetValue<string>("Data:PostgresConnectionString"));
                        })
                        .BuildServiceProvider();
        }

        private static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            var client = _services.GetRequiredService<DiscordSocketClient>();
            var logger = _services.GetRequiredService<LogService>();

            client.Log                                         += logger.LogAsync;
            _services.GetRequiredService<CommandService>().Log += logger.LogAsync;

            await client.LoginAsync(TokenType.Bot, _configuration.GetSection("Bot")["Token"]);
            await client.StartAsync();

            await _services.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }
    }
}
