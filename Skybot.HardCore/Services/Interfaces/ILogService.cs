// --------------------------------------------------------------------------------------------------------------------
// Filename : ILogService.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 14.05.2022
// Last Modified On : 17.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Services.Interfaces
{
    using Discord;

    public interface ILogService
    {
        Task LogAsync(LogSeverity severity, string source, string message, Exception? exception = null);

        Task LogAsync(LogMessage message);
    }
}
