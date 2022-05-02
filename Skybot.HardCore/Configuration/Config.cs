// --------------------------------------------------------------------------------------------------------------------
// Filename : Config.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 26.12.2021 22:12
// Last Modified On : 02.05.2022 17:49
// Copyrights : Copyright (c) Kristian Schlikow 2021-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Configuration;

using Newtonsoft.Json;

public class Config
{
    [JsonProperty("Bot")]
    public Bot Bot { get; set; }

    [JsonProperty("API")]
    public Api Api { get; set; }

    [JsonProperty("Discord")]
    public Discord Discord { get; set; }
}

public class Api
{
    [JsonProperty("URI")]
    public Uri Uri { get; set; }
}

public class Bot
{
    [JsonProperty("Token")]
    public string Token { get; set; }

    [JsonProperty("Trigger")]
    public string Trigger { get; set; }

    [JsonProperty("Name")]
    public string Name { get; set; }

    [JsonProperty("NLU")]
    public bool Nlu { get; set; }
}

public class Discord
{
    [JsonProperty("Guilds")]
    public List<ulong> Guilds { get; set; }

    [JsonProperty("Channels")]
    public List<ulong> Channels { get; set; }

    [JsonProperty("Roles")]
    public List<ulong> Roles { get; set; }
}
