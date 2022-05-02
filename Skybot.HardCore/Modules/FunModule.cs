// --------------------------------------------------------------------------------------------------------------------
// Filename : FunModule.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 01.05.2022
// Last Modified On : 02.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Modules
{
    using Discord.Commands;

    using Services;

    // Modules must be public and inherit from an IModuleBase
    public class FunModule : ModuleBase<SocketCommandContext>
    {
        public FunModule(PictureService pictureService) => PictureService = pictureService;

        private PictureService PictureService { get; }

        [Command("ping")]
        public Task PingAsync() => ReplyAsync("pong!");

        [Command("pong")]
        public Task PongAsync() => ReplyAsync("ping!");

        [Command("cat")]
        public async Task CatAsync()
        {
            // Get a stream containing an image of a cat
            var stream = await PictureService.GetCatPictureAsync();
            // Streams must be seeked to their beginning before being uploaded!
            stream.Seek(0, SeekOrigin.Begin);
            await Context.Channel.SendFileAsync(stream, "cat.png");
        }
    }
}
