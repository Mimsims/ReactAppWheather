using Microsoft.EntityFrameworkCore;
using ReactAppWheather.Server.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register service for fetching API data
builder.Services.AddHostedService<ReactAppWheather.Server.Controllers.WheatherService>();

// Add DBContext
builder.Services.AddDbContext<TemperatureRowDbContext>(
                    options => options.UseSqlServer(builder.Configuration.GetConnectionString(builder.Configuration.GetValue<string>("DBName"))),
                                                                                              ServiceLifetime.Singleton);
// Add API DBContext
builder.Services.AddDbContext<TemperatureAPIDbContext>(
                    options => options.UseSqlServer(builder.Configuration.GetConnectionString(builder.Configuration.GetValue<string>("DBName"))),
                                                                                              ServiceLifetime.Singleton);
// Add CORS 
builder.Services.AddCors(
                    options =>
                    {
                        options.AddPolicy("AllowLocalhost5173",
                            builder =>
                            {
                                builder.WithOrigins("https://localhost:5173")
                                       .AllowAnyHeader()
                                       .AllowAnyMethod();
                            });
                    });

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseCors("AllowLocalhost5173");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallbackToFile("/index.html");

app.Run();
