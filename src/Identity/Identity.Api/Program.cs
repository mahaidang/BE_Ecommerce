using Identity.Application.Auth;
using Identity.Application.Common;
using Identity.Infrastructure.Models;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// OpenAPI + Swagger UI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(o =>
{
    // hiển thị nút Authorize (Bearer)
    o.SwaggerDoc("v1", new() { Title = "Identity API", Version = "v1" });

    o.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Nhập: Bearer {token}"
    });
    o.AddSecurityRequirement(new()
    {
        {
            new() { Reference = new() { Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme, Id = "Bearer" } },
            Array.Empty<string>()
        }
    });
});

// EF
builder.Services.AddDbContext<IdentityDbContext>(opt =>
    opt.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<IIdentityDbContext>(sp => sp.GetRequiredService<IdentityDbContext>());

// MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<RegisterHandler>());

// JWT
var jwt = builder.Configuration.GetSection("Jwt");
var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["Key"]!));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new()
        {
            ValidIssuer = jwt["Issuer"],
            ValidAudience = jwt["Audience"],
            IssuerSigningKey = key,
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

var app = builder.Build();

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
                    Url = "http://localhost:5000/api/identity",
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


app.UseSwaggerUI(c =>
{
    // DÙNG ĐƯỜNG DẪN TƯƠNG ĐỐI để chạy được cả qua Gateway lẫn trực tiếp 5101
    c.SwaggerEndpoint("v1/swagger.json", "Identity API v1");
    c.RoutePrefix = "swagger";
});

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok("OK"));

// ---------- API qua MediatR ----------
app.MapPost("/auth/register",
    async (RegisterCommand cmd, IMediator mediator)
        => Results.Ok(await mediator.Send(cmd)));

app.MapPost("/auth/login",
    async (LoginCommand cmd, IMediator mediator) =>
    {
        var res = await mediator.Send(cmd);

        // tạo JWT ở API layer (Application chỉ trả LoginResult)
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, res.UserId.ToString()),
        new Claim(JwtRegisteredClaimNames.UniqueName, res.Username),
        new Claim(JwtRegisteredClaimNames.Email, res.Email),
        new Claim(ClaimTypes.NameIdentifier, res.UserId.ToString()),
        new Claim(ClaimTypes.Name, res.Username)
    };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: creds);

        var jwtString = new JwtSecurityTokenHandler().WriteToken(token);

        return Results.Ok(new
        {
            res.UserId,
            res.Username,
            res.Email,
            accessToken = jwtString,
            tokenType = "Bearer",
            expiresIn = 7200
        });
    });

// endpoint test token
app.MapGet("/me", (ClaimsPrincipal user) =>
{
    var sub = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue(JwtRegisteredClaimNames.Sub);
    var name = user.Identity?.Name ?? user.FindFirstValue(JwtRegisteredClaimNames.UniqueName);
    var email = user.FindFirstValue(JwtRegisteredClaimNames.Email);
    return Results.Ok(new { userId = sub, username = name, email });
})
.RequireAuthorization();

app.Run();