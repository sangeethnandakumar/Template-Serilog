using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        [HttpGet(Name = "GetWeatherForecast")]
        public IActionResult Get()
        {
            Log.Debug("Some sample logs");
            Log.Information("Some sample logs");
            Log.Warning("Some sample logs");
            try
            {
                throw new Exception("This is a test exception");
            }
            catch (Exception ex)
            {
                //Catch exceptions and write hints
                Log.Error("An exception occured.".AddHints(
                    new
                    {
                        A = "Value of A",
                        B = "Value of B",
                        C = new { Fname = "Sangee", LName = "Nandakumar" }
                    }
                    , ex));
            }
            finally
            {
            }
            return Ok(1);
        }
    }
}