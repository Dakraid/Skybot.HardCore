// --------------------------------------------------------------------------------------------------------------------
// Filename : FactoidModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 14.05.2022
// Last Modified On : 17.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules
{
    using Discord.Commands;

    using Microsoft.Extensions.Configuration;

    using Models.Configuration;

    using System.Text.RegularExpressions;

    public class FactoidModule : ModuleBase<SocketCommandContext>
    {
        private readonly IConfiguration _configuration;
        private readonly SkybotContext _databaseContext;
        private readonly HttpClient _httpClient;

        public FactoidModule(SkybotContext databaseContext, HttpClient httpClient, IConfiguration configuration)
        {
            _databaseContext = databaseContext;
            _httpClient = httpClient;
            _configuration = configuration;
        }

        [Command("AddFact")]
        [Summary("In progress.")]
        public Task AddFact(string content)
        {
            var text = content[1..];
            var match = Regex.Match(text, @"(\w+)\s(is)\s(.+)");

            return ReplyAsync($"{text} | Key: {match.Groups[1].Value} | Connector: {match.Groups[2].Value} | Content: {match.Groups[3].Value}");
        }
    }
}
