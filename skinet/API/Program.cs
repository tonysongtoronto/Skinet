using API.Middleware;
using Core.Interfaces;
using Infrastructure;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();


builder.Services.AddDbContext<StoreContext>(opt =>
{
    opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddCors();

// builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
// {
//     var connectionString = builder.Configuration.GetConnectionString("Redis")
//         ?? throw new Exception("Cannot get redis connection string");
//     var configuation = ConfigurationOptions.Parse(connectionString, true);
//     return ConnectionMultiplexer.Connect(configuation);

// });

builder.Services.AddSingleton<IConnectionMultiplexer>(config =>
{
    var logger = config.GetRequiredService<ILogger<Program>>();
    var connectionString = builder.Configuration.GetConnectionString("Redis")
        ?? throw new Exception("Cannot get redis connection string");

    logger.LogInformation("Redis连接字符串: {ConnectionString}", connectionString);

    var configuration = ConfigurationOptions.Parse(connectionString, true);
    var connection = ConnectionMultiplexer.Connect(configuration);

    logger.LogInformation("Redis连接状态: {IsConnected}", connection.IsConnected);

    return connection;
});

builder.Services.AddSingleton<ICartService, CartService>();



var app = builder.Build();
app.UseMiddleware<ExceptionMiddleware>();

app.UseCors(x => x
    .AllowAnyHeader()
    .AllowAnyMethod()
    .WithOrigins("http://localhost:4200", "https://localhost:4200"));

app.MapControllers();

try
{
    using var scope = app.Services.CreateScope();
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StoreContext>();
    await context.Database.MigrateAsync();
    await StoreContextSeed.SeedAsync(context);
     
}
catch (Exception e)
{
    Console.WriteLine(e);
    throw;
}

app.Run();
