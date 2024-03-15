using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ReactAppWheather.Server.Models
{
    public class ApiSettings
    {
        public string? BaseUri { get; set; }
        public string? DefaultLatitude { get; set; }
        public string? DefaultLongitude { get; set; }
        public string? DefaultParameters { get; set; }
        public double TimeSpan { get; set; } = 60.00;
    }

    public class WeatherApiResponse
    {
        [JsonPropertyName("latitude")]
        public float Latitude { get; set; }
        [JsonPropertyName("longitude")]
        public float Longitude { get; set; }
        [JsonPropertyName("generationtime_ms")]
        public float GenerationTimeMs { get; set; }
        [JsonPropertyName("utc_offset_seconds")]
        public int UtcOffsetSeconds { get; set; }
        [JsonPropertyName("timezone")]
        public string? Timezone { get; set; }
        [JsonPropertyName("timezone_abbreviation")]
        public string? TimezoneAbbreviation { get; set; }
        [JsonPropertyName("elevation")]
        public float? Elevation { get; set; }
        [JsonPropertyName("current_units")]
        public WeatherApiResponseCurrentUnits? CurrentUnits { get; set; }
        [JsonPropertyName("current")]
        public WeatherApiResponseCurrent? Current { get; set; }
    }

    public class WeatherApiResponseCurrentUnits
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }
        [JsonPropertyName("interval")]
        public string? Interval { get; set; }
        [JsonPropertyName("temperature")]
        public string? Temperature { get; set; }
    }

    public class WeatherApiResponseCurrent
    {
        [JsonPropertyName("time")]
        public string? Time { get; set; }
        [JsonPropertyName("interval")]
        public int? Interval { get; set; }
        [JsonPropertyName("temperature")]
        public float Temperature { get; set; }
    }

    public class TemperatureRow
    {
        public int Id { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public double? Temperature { get; set; }
        public DateTime? UpdateTimeStamp { get; set; }
    }

    public class CountryRow
    {
        public int Id { get; set; }
        public string? Name { get; set; }
    }
    
    public class CityRow 
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Latitude { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Longitude { get; set; }
        public int CountryId { get; set; } 
    }

    public class TemperatureRowDbContext : DbContext
    {
        public DbSet<TemperatureRow>? Temperatures { get; set; }
        public DbSet<CountryRow>? Countries { get; set; }
        public DbSet<CityRow>? Cities { get; set; }
        public TemperatureRowDbContext(DbContextOptions<TemperatureRowDbContext> options) : base(options)
        {
        }
    }

    public class TemperatureAPIDbContext : DbContext
    {
        public DbSet<TemperatureRow>? Temperatures { get; set; }
        public DbSet<CountryRow>? Countries { get; set; }
        public DbSet<CityRow>? Cities { get; set; }

        public TemperatureAPIDbContext(DbContextOptions<TemperatureAPIDbContext> options) : base(options)
        {
        }
    }

    public class WheatherFetchModel
    {
        private readonly HttpClient _client;
        private readonly IConfiguration _configuration;
        private readonly ApiSettings _apiSettings;

        private readonly TemperatureRowDbContext _context;

        public WheatherFetchModel(IConfiguration configuration, TemperatureRowDbContext context)
        {
            _configuration = configuration;
            _context = context;
            _apiSettings = _configuration.GetSection("ApiSettings").Get<ApiSettings>();
            _client = new HttpClient();
        }

        public string BuildApiUri(double latitude, double longitude)
        {
            string apiUri = $"{_apiSettings.BaseUri}/forecast?latitude={latitude}&longitude={longitude}&{_apiSettings.DefaultParameters}";
            return apiUri;
        }

        public void StartFetching()
        {
            var timer = new Timer(async (state) => {
                await UpdateWeatherData(); 
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(_apiSettings.TimeSpan));
        }

        private async Task UpdateWeatherData()
        {
            WeatherApiResponse? wheatherResponse;

            if (_context.Cities != null && _context.Temperatures != null)
            {
                var cities = await _context.Cities.ToListAsync();;
                foreach (var city in cities)
                {
                    var response = await _client.GetAsync(BuildApiUri(decimal.ToDouble(city.Latitude),
                                                                      decimal.ToDouble(city.Longitude)));
                    if (response.IsSuccessStatusCode)
                    {

                        var content = await response.Content.ReadAsStringAsync();
                        wheatherResponse = JsonSerializer.Deserialize<WeatherApiResponse>(content);
                        if (wheatherResponse != null)
                        {
                            var temperature = new TemperatureRow
                            {
                                CountryId = city.CountryId,
                                CityId = city.Id,
                                Temperature = wheatherResponse.Current?.Temperature ?? 0,
                                UpdateTimeStamp = DateTime.Now
                            };
                            await _context.Temperatures.AddAsync(temperature);
                        }
                    }

                }
                await _context.SaveChangesAsync();
            }  
        }
    }
}
