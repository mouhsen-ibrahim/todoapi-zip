using Microsoft.AspNetCore.Mvc;
using Datadog.Trace;
using Datadog.Trace.Configuration;
using StatsdClient;

namespace TodoApi.Controllers;

[ApiController]
[Route("[controller]")]
public class WeatherForecastController : ControllerBase
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    private readonly ILogger<WeatherForecastController> _logger;
    private StatsdConfig _dogstatsdConfig;

    public WeatherForecastController(ILogger<WeatherForecastController> logger)
    {
        _logger = logger;
        _dogstatsdConfig = new StatsdConfig
        {
            StatsdServerName = "127.0.0.1",
            StatsdPort = 8125,
        };

// Create a settings object using the existing
// environment variables and config sources
var settings = TracerSettings.FromDefaultSources();

// Override a value
settings.GlobalTags.Add("SomeKey", "SomeValue");

// Replace the tracer configuration
Tracer.Configure(settings);
    }

    [HttpGet(Name = "GetWeatherForecast")]
    public IEnumerable<WeatherForecast> Get()
    {
        using (var parentScope =
       Tracer.Instance.StartActive("manual.sortorders"))
{
    parentScope.Span.ResourceName = "GetWeatherForecast2";
    using (var childScope =
           Tracer.Instance.StartActive("manual.sortorders.child"))
    {
        using (var dogStatsdService = new DogStatsdService())
        {
            if (!dogStatsdService.Configure(_dogstatsdConfig))
                throw new InvalidOperationException("Cannot initialize DogstatsD. Set optionalExceptionHandler argument in the `Configure` method for more information.");

            
        
        // Nest using statements around the code to trace
        childScope.Span.ResourceName = "GetWeatherForecast2";
        dogStatsdService.Gauge("example_metric.gauge", Random.Shared.Next(0, 10), tags: new[] {"environment:local"});
        Console.WriteLine("custom metric sent");
        return Enumerable.Range(1, 5).Select(index => new WeatherForecast
        {
            Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            TemperatureC = Random.Shared.Next(-20, 55),
            Summary = Summaries[Random.Shared.Next(Summaries.Length)]
        })
        .ToArray();
        }
    }
}
    }
}
