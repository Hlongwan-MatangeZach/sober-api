using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SoberPath_API.Context;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddDbContext<Sober_Context>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddOpenApi();


// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        // For development, we allow all origins.
        // In production, this should be replaced with a specific list of allowed origins.
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    app.MapScalarApiReference();
    app.MapOpenApi();

}

app.UseRouting();

// Use the CORS policy. This must be placed after UseRouting and before UseAuthorization.
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();