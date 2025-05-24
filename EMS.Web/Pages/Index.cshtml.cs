using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Web.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IConfiguration config;
		private readonly EventService eventService;
		public IndexModel(IConfiguration _config,EventService _eventService)
		{
			config = _config;
			eventService = _eventService;
		}

		public string adminPath { get; set; }
		public List<EMAnnounce> Announcements { get; set; }
		public List<EMEvent> Events{ get; set; }


		public async Task OnGetAsync()
		{
			adminPath = config.GetValue<string>("ApiEvents");
			int userid = HttpContext.Session.GetInt32("UserID") ?? 0;
			EMService db = new EMService(config.GetConnectionString("emdb"));
			Announcements =db.GetAnnounce(count:3);
			Events = await eventService.GetEventsAsync(userid:userid,count:5);
		}
	}
}
