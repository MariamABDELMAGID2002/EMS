using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Admin.Pages
{
    public class EventTypeModel : PageModel
    {
		private readonly IConfiguration config;
		[BindProperty(SupportsGet = true)]
		public string Act { get; set; } = "";
		[BindProperty]
		public EMEventType EventType { get; set; }
		public List<EMEventType> EventTypes { get; set; }
		public string Err { get; set; } = "";
		public EventTypeModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public void OnGet()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			EventTypes = db.GetEventType();
			int.TryParse(Act, out int id);
			if (id == 0)
				EventType = new EMEventType();
			else
				EventType = db.GetEventType(id).FirstOrDefault() ?? new EMEventType();

		}
		public ActionResult OnPost()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			db.SaveEventType(EventType);
			EventTypes = db.GetEventType();
			return RedirectToPage("EventType", new
			{
				Act = (int?)null,
			});
		}
		public ActionResult OnPostDelete()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			EventTypes = db.GetEventType();
			int.TryParse(Act, out int id);

			int usedCount = db.GetUsedCount(EMService.UsedTypes.eventTypes, id);
			if (usedCount > 0)
			{
				Err = $"Can't delete this type because it used {usedCount} times";
				return Page();
			}
			db.DeleteEventType(id);
			return RedirectToPage("EventType", new
			{
				Act = (int?)null,
			});
		}
	}
}
