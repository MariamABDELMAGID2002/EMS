using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Admin.Pages
{
    public class EventModel : PageModel
    {
		private readonly IConfiguration config;
		[BindProperty(SupportsGet = true)]
		public string Act { get; set; } = "";
		[BindProperty]
		public EMEvent Event { get; set; }
		[BindProperty]
		public List<EMEventPrice> EventPrices { get; set; }
		public List<EMEvent> Events { get; set; }
		public SelectList EventTypeList{ get; set; }
		public IFormFile upFile { get; set; }
		public string Err { get; set; } = "";
		public EventModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public void OnGet()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			Events = db.GetEvent();
			EventTypeList = new SelectList(db.GetEventType(), "EventTypeID", "TypeName");
			int.TryParse(Act, out int id);
			if (id == 0)
				Event = new EMEvent();
			else
				Event = db.GetEvent(id).FirstOrDefault() ?? new EMEvent();

			EventPrices = db.GetEventPrice(id);

		}
		public ActionResult OnPost()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			if (Event.EventID != 0)
				Event.UpdatedAt = DateTime.Now;
			else
				Event.CreatedAt = DateTime.Now;

			db.SaveEvent(Event);
			foreach (var price in EventPrices)
			{
				price.EventID = Event.EventID;
				db.SaveEventPrice(price);
			}
			if (upFile != null && upFile.Length > 0)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "e"+ Event.EventID + ".jpg");
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					upFile.CopyTo(fileStream);
				}
			}


			Events = db.GetEvent();
			return RedirectToPage("Event", new
			{
				Act = (int?)null,
			});
		}
		public ActionResult OnPostDelete()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			int.TryParse(Act, out int id);

			int usedCount = db.GetUsedCount(EMService.UsedTypes.events, id);
			if (usedCount > 0)
			{
				TempData["Err"] = $"Can't delete this event because the event or one of its prices used {usedCount} times";
				return RedirectToPage("Event", new
				{
					Act = (int?)null,
				});
			}
			db.DeleteEvent(id);
			Events = db.GetEvent();

			return RedirectToPage("Event", new
			{
				Act = (int?)null,
			});
		}

	}
}
