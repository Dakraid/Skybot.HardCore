// --------------------------------------------------------------------------------------------------------------------
// Filename : LogService.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 30.04.2022
// Last Modified On : 17.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Services
{
    using Discord;

    using Interfaces;

    using Microsoft.Extensions.Configuration;

    public class LogService : ILogService
    {
        private readonly IConfiguration _configuration;

        public LogService(IConfiguration configuration) => _configuration = configuration;

        public Task LogAsync(LogSeverity severity, string source, string message, Exception? exception = null) => LogAsync(new LogMessage(severity, source, message, exception));

        public Task LogAsync(LogMessage message)
        {
            Enum.TryParse(typeof(LogSeverity), _configuration.GetValue<string>("Logger:LogLevel"), true, out var logSeverity);

            if (!_configuration.GetValue<bool>("Logger:IsEnabled") && message.Severity < (LogSeverity)(logSeverity ?? LogSeverity.Info))
            {
                return Task.CompletedTask;
            }

            switch (message.Severity)
            {
                case LogSeverity.Critical:
                case LogSeverity.Error:
                    Console.ForegroundColor = ConsoleColor.Red;

                    break;

                case LogSeverity.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;

                    break;

                case LogSeverity.Info:
                    Console.ForegroundColor = ConsoleColor.White;

                    break;

                case LogSeverity.Verbose:
                case LogSeverity.Debug:
                    Console.ForegroundColor = ConsoleColor.DarkGray;

                    break;

                default:
                    Console.ResetColor();

                    break;
            }
            Console.WriteLine($"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}");
            Console.ResetColor();

            var logFile = _configuration.GetValue<string>("Logger:LogToFile");

            if (!string.IsNullOrWhiteSpace(logFile))
            {
                File.AppendAllLinesAsync(logFile, new[]
                {
                    $"{DateTime.Now,-19} [{message.Severity,8}] {message.Source}: {message.Message} {message.Exception}"
                });
            }

            return Task.CompletedTask;
        }
    }
}
