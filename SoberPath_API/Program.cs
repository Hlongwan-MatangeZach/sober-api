using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SoberPath_API.Context;
using SoberPath_API.Hobs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<Sober_Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();
builder.Services.AddSignalR();

// CORS Configuration (BEFORE builder.Build())
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins(
    // Local development
    "http://localhost:5173",           // React web app
    "http://localhost:5174",
    "http://localhost:8081",

    // Android emulator (maps to host machine)
    "http://10.0.2.2:8081",            // Android emulator (accessing Metro on host)
    "http://10.0.0.2:8081",
    "http://172.16.98.246:8081",


    // Device or LAN access to host
    "http://172.16.98.246:5173",      // Frontend on network (HTTP)
    "http://172.16.98.246:5173",
    "http://172.16.98.246:5177",
    "http://172.16.98.246:5177",
    "http://172.16.98.246:8081",      // Metro/web debugger on host via IP
    "http://172.16.98.246:8082",

    // Optional access by IP
    "https://172.16.98.246:8081",      // Another host/device?

    // Loopback fallback
    "http://127.0.0.1:8081",
    "http://localhost:3000",
    "http://localhost:3000",
    "http://localhost:3001",
    "http://localhost:3002",
    "http://localhost:2025",
    "http://localhost:2026",
    "http://localhost:8081",       // Metro bundler (local dev)
    "http://172.16.98.246:8081", // Android emulator access
    "http://10.0.2.2:8081"

              )
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

var app = builder.Build();

// Middleware order is CRUCIAL
//app.UseHttpsRedirection();
app.UseCors("AllowAll");  // SINGLE CORS middleware AFTER UseHttpsRedirection
app.UseAuthorization();
app.MapControllers();
app.MapHub<Rehab_NotificationHub>("/rehab_notificationHub");
app.MapHub <SW_NotificationHub > ("/sw_notificationHub");

if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
}

app.Run();