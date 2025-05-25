using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EMS.Web.Pages
{
    public class CartModel : PageModel
    {
		private readonly IConfiguration config;

		[BindProperty]
		public List<VWItem> CartItems { get; set; }
		public EMEvent Event { get; set; }
		public List<EMEventPrice> EventPrices { get; set; }
		public SelectList TicketType { get; set; }
		public string Err { get; set; } = "";
		public CartModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public IActionResult OnGet()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			string[] ids = (HttpContext.Session.GetString("cart")??"").Split(",");
			if(ids.Length==0)
				return Page();
			CartItems = new List<VWItem>();
			
			foreach (string sid in ids)
			{
				int.TryParse(sid, out int eid);
				if (eid == 0)
					continue;

				Event = db.GetEvent(eid).FirstOrDefault();
				if (Event == null)
					continue;

				VWItem item = new VWItem();
				item.EventID = Event.EventID;
				item.Title = Event.Title;
				item.Price = 0;
				item.TicketTypeID = 0;
				item.Price = 0;
				item.UnitCount = 0;
				item.MaxQuota = Event.MaxQuota;
				item.AvailableQuota = Event.AvailableQuota;
				CartItems.Add(item);

			}

			//EventPrices = db.GetEventPrice(Event.EventID);
			TicketType = new SelectList(db.GetTicketType(), "TicketTypeID", "TypeName");
			

			return Page();
		}
		//public ActionResult OnPost()
		//{

		//	EMService db = new EMService(config.GetConnectionString("emdb"));
		//	if (Event.EventID != 0)
		//		Event.UpdatedAt = DateTime.Now;
		//	else
		//		Event.CreatedAt = DateTime.Now;

		//	db.SaveEvent(Event);
		//	foreach (var price in EventPrices)
		//	{
		//		price.EventID = Event.EventID;
		//		db.SaveEventPrice(price);
		//	}
		//	if (upFile != null && upFile.Length > 0)
		//	{
		//		var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "e" + Event.EventID + ".jpg");
		//		using (var fileStream = new FileStream(filePath, FileMode.Create))
		//		{
		//			upFile.CopyTo(fileStream);
		//		}
		//	}


		//	Events = db.GetEvent();
		//	return RedirectToPage("Event", new
		//	{
		//		Act = (int?)null,
		//	});
		//}
		//public ActionResult OnPostDelete()
		//{

		//	EMService db = new EMService(config.GetConnectionString("emdb"));
		//	int.TryParse(Act, out int id);

		//	int usedCount = db.GetUsedCount(EMService.UsedTypes.events, id);
		//	if (usedCount > 0)
		//	{
		//		TempData["Err"] = $"Can't delete this event because the event or one of its prices used {usedCount} times";
		//		return RedirectToPage("Event", new
		//		{
		//			Act = (int?)null,
		//		});
		//	}
		//	db.DeleteEvent(id);
		//	Events = db.GetEvent();

		//	return RedirectToPage("Event", new
		//	{
		//		Act = (int?)null,
		//	});
		//}
	}
}
