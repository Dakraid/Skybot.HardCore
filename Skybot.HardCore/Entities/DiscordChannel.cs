// --------------------------------------------------------------------------------------------------------------------
// Filename : DiscordChannel.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.05.2022 18:37
// Last Modified On : 02.05.2022 22:01
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Entities
{
    public class DiscordChannel
    {
        public Guid Id { get; set; }
        public decimal ChannelId { get; set; }
        public string ChannelName { get; set; } = null!;
    }
}
