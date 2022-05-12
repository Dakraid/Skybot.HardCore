// --------------------------------------------------------------------------------------------------------------------
// Filename : Program.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 26.12.2021
// Last Modified On : 02.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2021-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Services;

    using Skybot.HardCore.Models.Configuration.Configuration;

    public class Program
    {
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;

        private readonly DiscordSocketConfig _socketConfig = new()
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true
        };

        private static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            var context = _serviceProvider.GetRequiredService<SkybotContext>();
            context.Database.EnsureCreated();

            var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            var logger = _serviceProvider.GetRequiredService<LogService>();

            client.Log += logger.LogAsync;
            _serviceProvider.GetRequiredService<CommandService>().Log += logger.LogAsync;

            await client.LoginAsync(TokenType.Bot, _configuration.GetSection("Bot")["Token"]);
            await client.StartAsync();

            await _serviceProvider.GetRequiredService<CommandHandlingService>().InitializeAsync();

            await Task.Delay(Timeout.Infinite);
        }

        private Program()
        {
            _configuration = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json")
                             .AddUserSecrets<Program>()
                             .Build();

            _serviceProvider = ConfigureServices(_serviceCollection, _configuration);
        }

        public static IServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add services to the container.
            services.AddDbContext<SkybotContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnectionString")
            ));

            // Discord Services
            services.AddSingleton(configuration);
            services.AddSingleton<DiscordSocketClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton<CommandHandlingService>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<PictureService>();
            services.AddSingleton<LogService>();

            return services.BuildServiceProvider();
        }
    }
}
