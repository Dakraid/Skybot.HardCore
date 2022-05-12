// --------------------------------------------------------------------------------------------------------------------
// Filename : DiscordChannel.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.05.2022
// Last Modified On : 02.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Models
{
    using System.ComponentModel.DataAnnotations;

    public class DiscordChannel
    {
        [Key]
        public Guid ChannelId { get; set; }
        public decimal DiscordChannelId { get; set; }
        public string DiscordChannelName { get; set; } = null!;
    }
}