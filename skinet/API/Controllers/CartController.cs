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



    [HttpDelete("test-redis-delete/{cartId}")]
    public async Task<IActionResult> TestRedisDelete(string cartId)
    {
        var db = redis.GetDatabase();

        Log.Information($"尝试删除购物车: {cartId}");

        // 检查key是否存在
        bool exists = await db.KeyExistsAsync(cartId);

        if (exists)
        {
            bool deleted = await db.KeyDeleteAsync(cartId);
            if (deleted)
            {
                Log.Information($"成功删除购物车: {cartId}");
                return Ok($"成功删除购物车: {cartId}");
            }
            else
            {
                Log.Error($"删除失败: {cartId}");
                return BadRequest($"删除失败: {cartId}");
            }
        }



        Log.Warning($"购物车不存在: {cartId}");
        return NotFound($"购物车不存在: {cartId}");
    }



    [HttpDelete("test-redis-clear-all")]
    public async Task<IActionResult> TestRedisClearAll()
    {
        try
        {
            var server = redis.GetServer("localhost:6379");
            var db = redis.GetDatabase();

            // 获取所有购物车相关的键
            var keys = server.Keys(database: 0, pattern: "cart*").ToArray();

            Log.Information($"找到 {keys.Length} 个购物车键");

            if (keys.Any())
            {
                // 批量删除
                long deletedCount = await db.KeyDeleteAsync(keys);
                Log.Information($"成功删除 {deletedCount} 个购物车");
                return Ok($"成功删除 {deletedCount} 个购物车");
            }

            return Ok("没有找到购物车数据");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "删除购物车时发生错误");
            return StatusCode(500, $"删除失败: {ex.Message}");
        }
    }

    [HttpDelete("test-redis-flush")]
    public async Task<IActionResult> TestRedisFlush()
    {
        try
        {
            var server = redis.GetServer("localhost:6379");

            Log.Warning("正在清空整个Redis数据库");

            await server.FlushDatabaseAsync();

            Log.Information("Redis数据库已清空");
            return Ok("Redis数据库已清空");
        }
        catch (Exception ex)
        {
            Log.Error(ex, "清空Redis数据库时发生错误");
            return StatusCode(500, $"清空失败: {ex.Message}");
        }
    }




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