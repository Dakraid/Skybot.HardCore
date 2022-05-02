// --------------------------------------------------------------------------------------------------------------------
// Filename : Configuration.cs
// Project: Skybot.HardCore / Skybot.HardCore
// Author : Kristian Schlikow (kristian@schlikow.de)
// Created On : 02.05.2022 00:44
// Last Modified On : 02.05.2022 17:49
// Copyrights : Copyright (c) Kristian Schlikow 2022-2022, All Rights Reserved
// License: License is provided as described within the LICENSE file shipped with the project
// If present, the license takes precedence over the individual notice within this file
// --------------------------------------------------------------------------------------------------------------------

namespace Skybot.HardCore.Database;

using System.Data.Entity;

using EFCache;
using EFCache.Redis;

using Microsoft.Extensions.Configuration;

public class Configuration : DbConfiguration
{
    public Configuration(IConfiguration configuration)
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
