using System;
using System.Text.Json;
using Core.Entities;
using Core.Interfaces;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;

namespace Infrastructure.Services;

public class CartService(IConnectionMultiplexer redis, ILogger<CartService> logger) : ICartService
{

    private readonly IDatabase _database = redis.GetDatabase();
    public async Task<bool> DeleteCartAsync(string key)
    {
        return await _database.KeyDeleteAsync(key);
    }

    public async Task<ShoppingCart?> GetCartAsync(string key)
    {
        var data = await _database.StringGetAsync(key);

        return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<ShoppingCart>(data!);
    }

    public async Task<ShoppingCart?> SetCartAsync(ShoppingCart cart)
    {

        logger.LogInformation("正在写入Redis，Cart ID: {CartId}", cart.Id);


        var created = await _database.StringSetAsync(cart.Id,
            JsonSerializer.Serialize(cart), TimeSpan.FromDays(30));

        logger.LogInformation("Redis写入结果: {Created}", created);

          

        if (!created) return null;

        return await GetCartAsync(cart.Id);
    }
    

}
