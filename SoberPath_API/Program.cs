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
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();

// CORS Configuration - FIXED
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins", policy =>
    {
     

        policy
            .WithOrigins("http://localhost:2025")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials();  // Required for SignalR with authentication
    });
});

var app = builder.Build();

// Configure middleware
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();

// CORS must come after UseRouting but before UseAuthorization and endpoints
app.UseCors("AllowSpecificOrigins");

app.UseAuthorization();

// Endpoints
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
    endpoints.MapHub<NotificationHub>("/notificationHub");
});

app.Run();