using AI_Sorter_Backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sorter_Backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public IEnumerable<WeatherForecast> Get()
        {
            return Enumerable.Range(1, 7).Select(index => new WeatherForecast
            {
                Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                TemperatureC = Random.Shared.Next(-20, 55),
                Summary = Summaries[Random.Shared.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }   

    [ApiController]
    [Route("api/[controller]")]
    public class XMLTableController : ControllerBase
    {
        [HttpPost(Name = "PostXMLTable")]
        public IEnumerable<XML> Post()
        {
            return Enumerable.Range(1, 1).Select(index => new XML
            {
                Columns = 1,
                Rows = 1,
                Data = "Help me!"
            })
            .ToArray();
        }
    }
}
