// --------------------------------------------------------------------------------------------------------------------
// Filename : ExampleModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 01.05.2022 16:56
// Last Modified On : 02.05.2022 17:49
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules;

using Discord;
using Discord.Commands;

// Modules must be public and inherit from an IModuleBase
public class ExampleModule : ModuleBase<SocketCommandContext>
{
    // Get info on a user, or the user who invoked the command if one is not specified
    [Command("userinfo")]
    public async Task UserInfoAsync(IUser? user = null)
    {
        user ??= Context.User;

        await ReplyAsync(user.ToString());
    }

    // [Remainder] takes the rest of the command's arguments as one argument, rather than splitting every space
    [Command("echo")]
    public Task EchoAsync([Remainder] string text) // Insert a ZWSP before the text to prevent triggering other bots!
    {
        return ReplyAsync('\u200B' + text);
    }

    // 'params' will parse space-separated elements into a list
    [Command("list")]
    public Task ListAsync(params string[] objects)
    {
        return ReplyAsync("You listed: " + string.Join("; ", objects));
    }

    // Setting a custom ErrorMessage property will help clarify the precondition error
    [Command("guild_only")]
    [RequireContext(ContextType.Guild, ErrorMessage = "Sorry, this command must be ran from within a server, not a DM!")]
    public Task GuildOnlyCommand()
    {
        return ReplyAsync("Nothing to see here!");
    }
}
