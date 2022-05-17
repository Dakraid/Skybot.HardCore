// --------------------------------------------------------------------------------------------------------------------
// Filename : Program.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 26.12.2021
// Last Modified On : 14.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2021-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore
{
    using Discord;
    using Discord.Commands;
    using Discord.WebSocket;

    using Hangfire;
    using Hangfire.PostgreSql;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    using Models.Configuration;

    using Services;
    using Services.Interfaces;

    public class Program : IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceCollection _serviceCollection = new ServiceCollection();
        private readonly IServiceProvider _serviceProvider;
        private readonly DiscordSocketClient _client;

        private readonly DiscordSocketConfig _socketConfig = new()
        {
            LogLevel = LogSeverity.Info,
            MessageCacheSize = 100,
            GatewayIntents = GatewayIntents.AllUnprivileged | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true
        };

        private Program()
        {
            _configuration = new ConfigurationBuilder()
                             .AddJsonFile("appsettings.json")
                             .AddUserSecrets<Program>()
                             .Build();


            _client = new DiscordSocketClient(_socketConfig);

            _serviceProvider = ConfigureServices(_serviceCollection, _configuration, _client);
        }

        private static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        private async Task MainAsync()
        {
            var context = _serviceProvider.GetRequiredService<SkybotContext>();
            await context.Database.EnsureCreatedAsync();

            GlobalConfiguration.Configuration
                               .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
                               .UseColouredConsoleLogProvider()
                               .UseSimpleAssemblyNameTypeSerializer()
                               .UseRecommendedSerializerSettings()
                               .UsePostgreSqlStorage(_configuration.GetConnectionString("PostgreSQLConnectionString") + ";Search Path=Hangfire");

            var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
            var logger = _serviceProvider.GetRequiredService<ILogService>();

            client.Log += logger.LogAsync;
            _serviceProvider.GetRequiredService<CommandService>().Log += logger.LogAsync;

            await client.LoginAsync(TokenType.Bot, _configuration.GetSection("Bot")["Token"]);
            await client.StartAsync();

            await _serviceProvider.GetRequiredService<ICommandHandlingService>().InitializeAsync();

            client.Ready += Client_Ready; ;

            client.UserUpdated += Client_UserUpdated;
            client.UserJoined += Client_UserJoined; ;
            client.UserLeft += Client_UserLeft; ;

            client.ChannelUpdated += Client_ChannelUpdated;
            client.ChannelCreated += Client_ChannelCreated;
            client.ChannelDestroyed += Client_ChannelDestroyed;

            client.RoleUpdated += Client_RoleUpdated;
            client.RoleCreated += Client_RoleCreated;
            client.RoleDeleted += Client_RoleDeleted;

            await Task.Delay(Timeout.Infinite);
        }

        private async Task Client_Ready() => await _serviceProvider.GetRequiredService<SystemService>().InitializeAsync();

        private async Task Client_UserUpdated(SocketUser arg1, SocketUser arg2) => await _serviceProvider.GetRequiredService<SystemService>().ScanUsersAsync();
        private async Task Client_UserJoined(SocketGuildUser arg) => await _serviceProvider.GetRequiredService<SystemService>().ScanUsersAsync();
        private async Task Client_UserLeft(SocketGuild arg1, SocketUser arg2) => await _serviceProvider.GetRequiredService<SystemService>().ScanUsersAsync();

        private async Task Client_ChannelUpdated(SocketChannel arg1, SocketChannel arg2) => await _serviceProvider.GetRequiredService<SystemService>().ScanChannelsAsync();
        private async Task Client_ChannelCreated(SocketChannel arg) => await _serviceProvider.GetRequiredService<SystemService>().ScanChannelsAsync();
        private async Task Client_ChannelDestroyed(SocketChannel arg) => await _serviceProvider.GetRequiredService<SystemService>().ScanChannelsAsync();

        private async Task Client_RoleUpdated(SocketRole arg1, SocketRole arg2) => await _serviceProvider.GetRequiredService<SystemService>().ScanRolesAsync();
        private async Task Client_RoleCreated(SocketRole arg) => await _serviceProvider.GetRequiredService<SystemService>().ScanRolesAsync();
        private async Task Client_RoleDeleted(SocketRole arg) => await _serviceProvider.GetRequiredService<SystemService>().ScanRolesAsync();

        private IServiceProvider ConfigureServices(IServiceCollection services, IConfiguration configuration, DiscordSocketClient client)
        {
            // Add services to the container.
            services.AddDbContext<SkybotContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("PostgreSQLConnectionString")));

            // Discord Services
            services.AddSingleton(configuration);
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<HttpClient>();
            services.AddSingleton<CommandService>();
            services.AddSingleton(client);
            services.AddSingleton<ICommandHandlingService, CommandHandlingService>();
            services.AddSingleton<PermissionService>();
            services.AddSingleton<PictureService>();
            services.AddSingleton<SystemService>();

            return services.BuildServiceProvider();
        }

        public async void Dispose()
        {
            await _client.DisposeAsync();
            await _serviceProvider.GetRequiredService<SkybotContext>().DisposeAsync();
        }
    }
}
