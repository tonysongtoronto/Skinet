using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;

using StackExchange.Redis;


namespace API.Controllers;

public class CartController(ICartService cartService,
IConnectionMultiplexer redis,
ILogger<CartController> logger) : BaseApiController
{

    [HttpGet("test-redis-read/{cartId}")]
    public async Task<IActionResult> TestRedisRead(string cartId)
    {
        var db = redis.GetDatabase();

         logger.LogError("Redis连接字符串: LogError");

          logger.LogDebug("Redis连接字符串: LogDebug");

        // 检查key是否存在
        bool exists = await db.KeyExistsAsync(cartId);
        logger.LogInformation("Redis连接字符串: {ConnectionString}", redis.Configuration);

        if (exists)
        {
            var value = await db.StringGetAsync(cartId);
            return Ok($"找到数据: {value}");
        }

        return NotFound("Redis中没有找到该key");
    }

[HttpGet("test-redis-keys")]
public async Task<IActionResult> TestRedisKeys()
{

     logger.LogError("Redis连接字符串: LogError");

    logger.LogDebug("Redis连接字符串: LogDebug");
    var server = redis.GetServer("localhost:6379");
    var keys = server.Keys(database: 0).ToList();
    
    return Ok($"Redis中的所有keys: {string.Join(", ", keys)}");
}



    [HttpGet]
    public async Task<ActionResult<ShoppingCart>> GetCartById(string id)
    {
        var cart = await cartService.GetCartAsync(id);

        return Ok(cart ?? new ShoppingCart { Id = id });
    }

    [HttpPost]
    public async Task<ActionResult<ShoppingCart>> UpdateCart(ShoppingCart cart)
    {
        var updatedCart = await cartService.SetCartAsync(cart);

        return Ok(updatedCart);
    }

    [HttpDelete]
    public async Task DeleteCart(string id)
    {
        await cartService.DeleteCartAsync(id);
    }
    

}