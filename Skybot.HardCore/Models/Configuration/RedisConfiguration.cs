// --------------------------------------------------------------------------------------------------------------------
// Filename : Configuration.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.05.2022
// Last Modified On : 02.05.2022
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Models.Configuration
{
    using EFCache;
    using EFCache.Redis;

    using Microsoft.Extensions.Configuration;

    using Skybot.HardCore.Models.Configuration.Configuration;

    using System.Data.Entity;

    public class RedisConfiguration : DbConfiguration
    {
        public RedisConfiguration(IConfiguration configuration)
        {
            var redisConnection = configuration.GetValue<string>("Data:RedisConnectionString");
            var cache = new RedisCache(redisConnection);
            var transactionHandler = new CacheTransactionHandler(cache);
            AddInterceptor(transactionHandler);

            Loaded += (sender, args) =>
            {
                args.ReplaceService<CachingProviderServices>((s, _) => new CachingProviderServices(s, transactionHandler, new RedisCachingPolicy()));
            };
        }
    }
}
