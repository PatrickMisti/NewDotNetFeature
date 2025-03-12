using System.Threading.Channels;
using Microsoft.AspNetCore.Mvc;
using Student_Coordinator.Message;

namespace Channel_Backworker.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private readonly Channel<RequestBackgroundService> _channel;

    public WeatherForecastController(ILogger<WeatherForecastController> logger, Channel<RequestBackgroundService> c)
    {
        _logger = logger;
        _channel = c;
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public async Task<IEnumerable<WeatherForecast>> Get()
    {
        await _channel.Writer.WriteAsync(new RequestBackgroundService { Message = "Hello from WeatherForecastController" });
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
    }
}
