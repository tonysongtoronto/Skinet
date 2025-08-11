using System;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;

using StackExchange.Redis;


namespace API.Controllers;

public class CartController(ICartService cartService,
IConnectionMultiplexer redis
) : BaseApiController
{

    [HttpGet("test-redis-read/{cartId}")]
    public async Task<IActionResult> TestRedisRead(string cartId)
    {
        var db = redis.GetDatabase();

  

         Log.Information("Fetching all products");

           Log.Debug("Fetching all products Debug");

             Log.Error("Fetching all products Error");

     

        // 检查key是否存在
        bool exists = await db.KeyExistsAsync(cartId);
     

        if (exists)
        {
            var value = await db.StringGetAsync(cartId);
            return Ok($"找到数据: {value}");
        }

        return NotFound("Redis中没有找到该key");
    }

[HttpGet("test-redis-keys")]
public  IActionResult TestRedisKeys()
{

         Log.Information("test-redis-keys");

           Log.Debug("test-redis-keys Debug");

             Log.Error("test-redis-keys Error");


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