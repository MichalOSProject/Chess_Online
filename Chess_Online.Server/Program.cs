using Chess_Online.Server.Data;
using Microsoft.EntityFrameworkCore;
using Chess_Online.Server.Services.Interfaces;
using Chess_Online.Server.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy =>
        {
            policy.WithOrigins("https://localhost:5173")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddScoped<IGameInstanceService, GameInstanceService>();
builder.Services.AddScoped<IGameActionService, GameActionService>();
builder.Services.AddScoped<IDataConversionService, DataConversionService>();
builder.Services.AddScoped<IAuthService,AuthService>();
builder.Services.AddScoped<ILobbyService, LobbyService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.UseWebSockets();

app.MapGet("/api/GameWS", async (HttpContext context) =>
{
    var gameActionService = context.RequestServices.GetRequiredService<IGameActionService>();

    if (context.WebSockets.IsWebSocketRequest)
    {
        var token = context.Request.Query["token"].ToString();
        Console.WriteLine("WebSocket connection request received");
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await gameActionService.GameAction(webSocket,token);
    }
    else
    {
        Console.WriteLine("Not a WebSocket request");
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Expected a WebSocket request");
    }
});

app.MapGet("/api/lobby", async (HttpContext context) =>
{
    var lobbyService = context.RequestServices.GetRequiredService<ILobbyService>();

    if (context.WebSockets.IsWebSocketRequest)
    {
        var token = context.Request.Query["token"].ToString();
        Console.WriteLine("WebSocket connection request received");
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        await lobbyService.HandleWebSocketAsync(webSocket, token);
    }
    else
    {
        Console.WriteLine("Not a WebSocket request");
        context.Response.StatusCode = 400;
        await context.Response.WriteAsync("Expected a WebSocket request");
    }
});


app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
