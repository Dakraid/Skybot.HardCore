// --------------------------------------------------------------------------------------------------------------------
// Filename : Factoid.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 14.05.2022
// Last Modified On : 14.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Models
{
    using System.ComponentModel.DataAnnotations;

    public class Factoid
    {
        [Key]
        public Guid FactId { get; set; }

        [Required]
        public string FactKey { get; set; }

        [Required]
        public string FactContent { get; set; }

        [Required]
        public DiscordUser UserId { get; set; }
    }
}
