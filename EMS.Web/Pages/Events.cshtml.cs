using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Web.Pages
{
    public class EventsModel : PageModel
    {
		private readonly WeatherService weatherService;
		private readonly EventService eventService;

		public WeatherForecast Forecast { get; set; }
		public List<EMEvent> Events { get; set; }

		public EventsModel(WeatherService _weatherService,EventService _eventService)
		{
			weatherService = _weatherService;
			eventService = _eventService;
		}

		public async Task OnGetAsync()
		{
			Forecast = await weatherService.GetWeatherAsync(41.0217, 28.9338);
			Events = await eventService.GetEventsAsync();
		}
	}
}
