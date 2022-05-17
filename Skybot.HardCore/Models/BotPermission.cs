namespace Skybot.HardCore.Models
{
    using Enums;

    using Microsoft.EntityFrameworkCore;

    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

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
