using EMS.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Web.Pages
{
    public class EventDetailsModel : PageModel
    {
		private readonly IConfiguration config;
		private readonly EventService eventService;
		private readonly WeatherService weatherService;
		public EventDetailsModel(IConfiguration _config, WeatherService _weatherService,EventService _eventService)
		{
			config = _config;
			eventService = _eventService;
			weatherService = _weatherService;
		}


		public WeatherForecast Forecast { get; set; }
		public string adminPath { get; set; }
		public EMEvent Event { get; set; }
		public EMEventType EventType { get; set; }


		public async Task OnGetAsync(int id)
		{
			adminPath = config.GetValue<string>("ApiEvents");
			EMService db = new EMService(config.GetConnectionString("emdb"));
			Event = (await eventService.GetEventsAsync(eventid: id)).FirstOrDefault();
			if (Event == null)
			{
				RedirectToPage("/Index");
			}
			EventType = db.GetEventType(Event.EventTypeID.Value).FirstOrDefault();

			Forecast = await weatherService.GetWeatherAsync(double.Parse(Event.latitude),double.Parse(Event.longitude));

			
		}
		public async Task OnPostAsync(int id)
		{
			string cart = HttpContext.Session.GetString("cart")??"";
			cart += "," + id;
			HttpContext.Session.SetString("cart",cart);

			adminPath = config.GetValue<string>("ApiEvents");
			EMService db = new EMService(config.GetConnectionString("emdb"));
			Event = (await eventService.GetEventsAsync(eventid: id)).FirstOrDefault();
			if (Event == null)
			{
				RedirectToPage("/Index");
			}
			EventType = db.GetEventType(Event.EventTypeID.Value).FirstOrDefault();

			Forecast = await weatherService.GetWeatherAsync(double.Parse(Event.latitude), double.Parse(Event.longitude));

		}
	}
}
