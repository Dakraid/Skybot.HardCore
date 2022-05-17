// --------------------------------------------------------------------------------------------------------------------
// Filename : BotPermission.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 16.05.2022
// Last Modified On : 17.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Models
{
    using Enums;

    using System.ComponentModel.DataAnnotations;

    public class BotPermission
    {
        [Key]
        public Guid PermissionId { get; set; }

        [Required]
        public string Command { get; set; }

        public List<DiscordUser> DiscordUsers { get; set; } = new();

        public List<DiscordRole> DiscordRoles { get; set; } = new();

        public SpecialPermission SpecialPermission { get; set; } = SpecialPermission.AnyRole;
    }
}
