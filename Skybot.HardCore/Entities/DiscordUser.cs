// --------------------------------------------------------------------------------------------------------------------
// Filename : DiscordUser.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.05.2022 00:36
// Last Modified On : 02.05.2022 22:03
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Entities
{
    public class DiscordUser
    {
        public Guid Id { get; set; }
        public decimal UserId { get; set; }
        public string UserDisplayName { get; set; } = null!;
        public decimal UserDiscriminator { get; set; }
        public bool IsBlocked { get; set; }
    }
}
