using BasketService.Application.Interfaces;
using BasketService.Infrastructure;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using Microsoft.Extensions.DependencyInjection; // Add this using directive
using Swashbuckle.AspNetCore.SwaggerGen; // Add this using directive
using Swashbuckle.AspNetCore.SwaggerUI; // Add this using directive

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    o.SwaggerDoc("v1", new OpenApiInfo { Title = "Basket API", Version = "v1" });
});

builder.Services.AddHttpClient("ProductApi", (sp, c) =>
{
    var cfg = sp.GetRequiredService<IConfiguration>();
    var baseUrl = cfg["Services:ProductBaseUrl"]!;
    c.BaseAddress = new Uri(baseUrl);
    c.Timeout = TimeSpan.FromSeconds(5);
});


// Redis
var redisConn = builder.Configuration["Redis:ConnectionString"]!;
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisConn));

// Repo
builder.Services.AddSingleton<IBasketRepository, RedisBasketRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(c =>
    {
        c.PreSerializeFilters.Add((doc, req) =>
        {
            // Detect nếu request đến từ Gateway (qua port 5000)
            var isViaGateway = req.Host.Port == 5000 ||
                              req.Headers.ContainsKey("X-Forwarded-Prefix") ||
                              req.Headers["Referer"].ToString().Contains(":5000");

            if (isViaGateway)
            {
                // Force URL qua Gateway
                doc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = "http://localhost:5000/api/basket",
                    Description = "Via Gateway"
                }
            };
            }
            else
            {
                // Chạy trực tiếp service
                doc.Servers = new List<OpenApiServer>
            {
                new OpenApiServer
                {
                    Url = $"{req.Scheme}://{req.Host.Value}",
                    Description = "Direct Access"
                }
            };
            }
        });
    });
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
#region BasketService.Api/Program.cs (DEV ONLY)
//app.MapGet("/bench/redis", async (StackExchange.Redis.IConnectionMultiplexer mux) =>
//{
//    var db = mux.GetDatabase();
//    var key = "bench:key";
//    var payload = new string('x', 256);

//    var sw = System.Diagnostics.Stopwatch.StartNew();
//    const int N = 10_000;

//    // SET tuần tự
//    for (int i = 0; i < N; i++) await db.StringSetAsync($"{key}:{i}", payload);
//    sw.Stop();
//    var setMs = sw.ElapsedMilliseconds;

//    // GET tuần tự
//    sw.Restart();
//    for (int i = 0; i < N; i++) _ = await db.StringGetAsync($"{key}:{i}");
//    sw.Stop();
//    var getMs = sw.ElapsedMilliseconds;

//    // GET song song (throttle 100)
//    sw.Restart();
//    var throttler = new System.Threading.SemaphoreSlim(100);
//    var tasks = Enumerable.Range(0, N).Select(async i => {
//        await throttler.WaitAsync();
//        try { _ = await db.StringGetAsync($"{key}:{i}"); }
//        finally { throttler.Release(); }
//    });
//    await Task.WhenAll(tasks);
//    sw.Stop();
//    var getParMs = sw.ElapsedMilliseconds;

//    return Results.Ok(new
//    {
//        notes = "DEV benchmark only – do not run in production",
//        N,
//        set_ms = setMs,
//        set_req_per_sec = (int)(N / (setMs / 1000.0)),
//        get_ms = getMs,
//        get_req_per_sec = (int)(N / (getMs / 1000.0)),
//        get_parallel_ms = getParMs,
//        get_parallel_req_per_sec = (int)(N / (getParMs / 1000.0))
//    });
//});
#endregion

app.Run();
