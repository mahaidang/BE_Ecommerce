var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

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

app.MapReverseProxy();
app.MapGet("/health", () => Results.Ok("OK"));
app.Run();

// Class để binding configuration
public class SwaggerEndpoint
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}