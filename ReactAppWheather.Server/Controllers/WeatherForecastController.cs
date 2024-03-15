using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReactAppWheather.Server.Models;

namespace ReactAppWheather.Server.Controllers
{
    public class WheatherService : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly WheatherFetchModel _wheatherFetchModel;

        public WheatherService(IConfiguration configuration, TemperatureRowDbContext context)
        {
            _configuration = configuration;
            _wheatherFetchModel = new(_configuration, context);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _wheatherFetchModel.StartFetching();
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }

    public class TemperatureData
    {
        public string? CountryName { get; set; }
        public int? CountryId { get; set; }
        public List<CityData>? Cities { get; set; }
    }

    public class CityData
    {
        public string? CityName { get; set; }
        public List<TemperatureInfo>? Temperatures { get; set; }
    }

    public class TemperatureInfo
    {
        public double? Temperature { get; set; }
        public DateTime? UpdateTimeStamp { get; set; }
    }

    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly TemperatureAPIDbContext _context;

        public WeatherForecastController(TemperatureAPIDbContext context)
        {
            _context = context;
        }

        [HttpGet("temperatures")]
        public async Task<IActionResult> TemperaturesDataAsync()
        {
            if (_context.Countries != null && await _context.Countries.AnyAsync()
                && _context.Cities != null && await _context.Cities.AnyAsync())
            {
                var temperaturesData = await (from t in _context.Temperatures
                                              join c in _context.Countries on t.CountryId equals c.Id
                                              join ct in _context.Cities on t.CityId equals ct.Id
                                              orderby c.Name, ct.Name, t.UpdateTimeStamp
                                              select new { CountryName = c.Name, CityName = ct.Name, t.Temperature, t.UpdateTimeStamp })
                    .ToListAsync();

                var formattedData = temperaturesData
                        .GroupBy(td => td.CountryName)
                        .Select(group => new
                        {
                            CountryName = group.Key,
                            CityNames = group
                                .GroupBy(c => c.CityName)
                                .Select(groupCity => new
                                {
                                    CityName = groupCity.Key,
                                    Temperatures = groupCity
                                                    .Select(groupTemp => new
                                                    {
                                                        groupTemp.Temperature,
                                                        groupTemp.UpdateTimeStamp
                                                    })
                                })
                        }); 

                return Ok(formattedData);
            }
            else
            {
                return BadRequest();
            }
        }
    }
}
