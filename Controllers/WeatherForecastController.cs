using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace WebApplication1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly RepositoryBase _repository;

        public WeatherForecastController(TransientRepository repository)
        //public WeatherForecastController(TransientRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public ActionResult Get() => this.PhysicalFile(Path.Join(AppDomain.CurrentDomain.BaseDirectory, "test.html"), "text/html");

        [HttpPost("bulkInsert")]
        public ActionResult BulkInsert()
        {
            var items = CreateItems();
            this._repository.BulkInsert(items, nameof(WeatherForecast));
            return this.NoContent();
        }

        [HttpGet("data")]
        public ObjectResult GetItems()
        {
            return this.Ok(this._repository.GetData<WeatherForecast>(nameof(WeatherForecast)));
        }

        [HttpPost("setup")]
        public ActionResult Setup()
        {
            _repository.SetupLocalDb();
            return this.NoContent();
        }

        [HttpPost("tearDown")]
        public ActionResult TearDown()
        {
            _repository.TeardownLocalDb();
            return this.NoContent();
        }

        private static IEnumerable<WeatherForecast> CreateItems()
        {
            var rng = new Random();
            return Enumerable.Range(1, 25).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }
    }
}
