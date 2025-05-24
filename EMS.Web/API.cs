using EMS.Data;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
namespace EMS.Web
{
	public class WeatherForecast
	{
		public double Latitude { get; set; }
		public double Longitude { get; set; }
		public double Elevation { get; set; }

		[JsonPropertyName("current_weather")]
		public CurrentWeather Current { get; set; }
	}

	
	public class CurrentWeather
	{
		public double Temperature { get; set; }
		public double Windspeed { get; set; }
		public int Winddirection { get; set; }
		public int Weathercode { get; set; }
		public string Time { get; set; }
	}

	public class WeatherService
	{
		private readonly HttpClient _httpClient;
		private readonly IConfiguration config;

		public WeatherService(HttpClient httpClient,IConfiguration _config)
		{
			_httpClient = httpClient;
			config = _config;
		}

		public async Task<WeatherForecast> GetWeatherAsync(double latitude, double longitude)
		{
			var url = config.GetValue<string>("ApiWeather")+ $"forecast?latitude={latitude}&longitude={longitude}&current_weather=true";

			try
			{
				var response = await _httpClient.GetFromJsonAsync<WeatherForecast>(url);
				return response;
			}
			catch (HttpRequestException ex)
			{
				// Log error or handle as needed
				throw new Exception("Error fetching weather data", ex);
			}
		}
	}

	public class EventService
	{
		private readonly HttpClient _httpClient;
		IConfiguration config;

		public EventService(HttpClient httpClient,IConfiguration _config)
		{
			_httpClient = httpClient;
			config = _config;
		}

		public async Task<List<EMEvent>> GetEventsAsync(int eventid=0, int userid=0,int count=0)
		{
			var url = config.GetValue<string>("ApiEvents") + $"api/GetEvents?eventid={eventid}&userid={userid}&count={count}";

			try
			{
				var response = await _httpClient.GetFromJsonAsync<List<EMEvent>>(url);
				return response;
			}
			catch (HttpRequestException ex)
			{
				// Log error or handle as needed
				throw new Exception("Error fetching event data", ex);
			}
		}
	}
}
