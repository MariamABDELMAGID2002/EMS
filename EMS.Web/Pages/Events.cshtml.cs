using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Web.Pages
{
    public class EventsModel : PageModel
    {

		private readonly EventService eventService;
		private readonly IConfiguration config;
		
		public List<EMEvent> Events { get; set; }
		public List<EMEventType> EventTypes { get; set; }
		public string adminPath { get; set; }
		public EventsModel(IConfiguration _config,EventService _eventService)
		{
			eventService = _eventService;
			config = _config;
		}

		public async Task OnGetAsync()
		{
			adminPath = config.GetValue<string>("ApiEvents");
			Events = await eventService.GetEventsAsync();
			var db = new EMService(config.GetConnectionString("emdb"));
			EventTypes = db.GetEventType();
		}
	}
}
