using ApiGateway.Api.Middlewares;

var builder = WebApplication.CreateBuilder(args);

//// ========== CONFIGURATION ==========
//builder.Configuration.AddJsonFile("yarp.json", optional: false, reloadOnChange: true);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// CORS (cho phép FE truy cập)
builder.Services.AddCors(opt => opt.AddDefaultPolicy(p =>
    p.WithOrigins("http://localhost:3000", "http://localhost:5173")
     .AllowAnyHeader().AllowAnyMethod().AllowCredentials()));

// Logging providers (Console + Debug)
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders();
    logging.AddConsole();
    logging.AddDebug();
});

var app = builder.Build();

// 1️⃣ CORS (phải trước ReverseProxy)
app.UseCors();

// 2️⃣ Custom middleware
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<ErrorHandlingMiddleware>();

// 3️⃣ Swagger UI đa endpoint
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    // Đọc danh sách endpoints từ appsettings
    var endpoints = builder.Configuration.GetSection("SwaggerEndpoints")
        .Get<List<SwaggerEndpoint>>() ?? new List<SwaggerEndpoint>();

    foreach (var endpoint in endpoints)
    {
        c.SwaggerEndpoint(endpoint.Url, endpoint.Name);
    }

    c.RoutePrefix = "swagger"; // => /swagger/index.html
    // c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.List); // nếu muốn sổ sẵn
});

// 4️⃣ Reverse proxy
app.MapReverseProxy();

// 5️⃣ Health check
app.MapGet("/health", () => Results.Ok("OK - Gateway"));
app.MapGet("/", () => Results.Ok("✅ API Gateway running"));

app.Run();

// ========== SUPPORT CLASS ==========
public class SwaggerEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}