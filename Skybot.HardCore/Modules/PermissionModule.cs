// --------------------------------------------------------------------------------------------------------------------
// Filename : PermissionModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 17.05.2022
// Last Modified On : 17.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules
{
    using Discord.Commands;

    using Models.Configuration;

    using Services;

    [Group("perms")]
    public class PermissionModule : ModuleBase<SocketCommandContext>
    {
        private readonly SkybotContext _databaseContext;
        private readonly PermissionService _permissions;
        private readonly SystemService _system;

        public PermissionModule(SkybotContext databaseContext, PermissionService permissions, SystemService system)
        {
            _databaseContext = databaseContext;
            _permissions = permissions;
            _system = system;
        }

        [Command("grant")]
        [Summary("Grant permissions to a command.")]
        public async Task GrantAsync()
        {
            if (!await _permissions.VerifyPermission(Context, "permsGrant"))
            {
                await ReplyAsync("You have no permissions to access this command.");
            }
        }

        [Command("revoke")]
        [Summary("Revoke permissions to a command.")]
        public async Task RevokeAsync()
        {
            if (!await _permissions.VerifyPermission(Context, "permsRevoke"))
            {
                await ReplyAsync("You have no permissions to access this command.");
            }
        }

        [Command("list")]
        [Summary("List the currently set permissions for a command.")]
        public async Task ListAsync(
            [Summary("The targeted command. If the command belongs to a group, both terms are written together without space.")]
            string command
        )
        {
            if (!await _permissions.VerifyPermission(Context, "permsList"))
            {
                await ReplyAsync("You have no permissions to access this command.");

                return;
            }

            var response = await _permissions.ListPermissions(Context, command);

            await ReplyAsync(response);
        }
    }
}
