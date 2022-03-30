using Catalago.Api.Context;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container --> Semelhante ao ConfigureServices da class Startup net5
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Cenexao ao Context BD
var ConectaBD = builder.Configuration.GetConnectionString("OlifransConnection");
builder.Services.AddDbContext<OlifransDbContext>(options => options.UseNpgsql(ConectaBD));








var app = builder.Build();

// Configure the HTTP request pipeline --> Semelhante ao Configure da class Startup net5
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
       new WeatherForecast
       (
           DateTime.Now.AddDays(index),
           Random.Shared.Next(-20, 55),
           summaries[Random.Shared.Next(summaries.Length)]
       ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

internal record WeatherForecast(DateTime Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}