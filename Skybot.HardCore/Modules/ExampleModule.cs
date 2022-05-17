// --------------------------------------------------------------------------------------------------------------------
// Filename : ExampleModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 01.05.2022
// Last Modified On : 14.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules
{
    using Discord;
    using Discord.Commands;

    // Modules must be public and inherit from an IModuleBase
    public class ExampleModule : ModuleBase<SocketCommandContext>
    {
        // Get info on a user, or the user who invoked the command if one is not specified
        [Command("userinfo")]
        [Summary("Provides info about the user.")]
        public async Task UserInfoAsync(IUser? user = null)
        {
            user ??= Context.User;

            await ReplyAsync(user.ToString());
        }

        // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
        [Command("echo")]
        [Summary("Echoes the input.")]
        public Task EchoAsync([Remainder] string text) // Insert a ZWSP before the text to prevent triggering other bots!
            =>
                ReplyAsync('\u200B' + text);
    }
}
