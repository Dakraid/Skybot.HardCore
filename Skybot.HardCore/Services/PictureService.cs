// --------------------------------------------------------------------------------------------------------------------
// Filename : PictureService.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 30.04.2022
// Last Modified On : 14.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Services
{
    public class PictureService
    {
        private readonly HttpClient _http;

        public PictureService(HttpClient http) => _http = http;

        public async Task<Stream> GetCatPictureAsync()
        {
            var resp = await _http.GetAsync("https://cataas.com/cat");

            return await resp.Content.ReadAsStreamAsync();
        }
    }
}
