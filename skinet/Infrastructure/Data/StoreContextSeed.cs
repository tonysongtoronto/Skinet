using System;
using System.Reflection;
using System.Text.Json;
using Core.Entities;

namespace Infrastructure.Data;

public class StoreContextSeed
{
    public static async Task SeedAsync(StoreContext context)
    {
        if (!context.Products.Any())
        {

            //     var basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            //   var filePath = Path.Combine(basePath!, "Infrastructure", "Data", "SeedData", "products.json");


            // var productsData = await File.ReadAllTextAsync(filePath);

            var productsData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);

            if (products == null) return;

            context.Products.AddRange(products);

            await context.SaveChangesAsync();
        }
        
        
        if (!context.DeliveryMethods.Any())
        {
            var dmData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");
            var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

            if (methods == null) return;

            context.DeliveryMethods.AddRange(methods);

            await context.SaveChangesAsync();
        }
    }
}
