using EMS.Data;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography.X509Certificates;

namespace EMS.Web.Pages
{
    public class CartModel : PageModel
    {
		private readonly IConfiguration config;

		[BindProperty]
		public EMOrder Order { get; set; }
		[BindProperty]
		public List<EMOrderItem> CartItems { get; set; }
		public List<EMEventPrice> EventPrices { get; set; }
		public SelectList TicketType { get; set; }

		public CartModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public IActionResult OnGet()
		{
			int userid = HttpContext.Session.GetInt32("UserID") ?? 0;
			if (userid == 0)
			{
				RedirectToPage("Sign");
			}
			EMService db = new EMService(config.GetConnectionString("emdb"));
			string[] ids = (HttpContext.Session.GetString("cart")??"").Split(",");
			if(ids.Length==0)
				return Page();
			CartItems = new List<EMOrderItem>();
			Order = new EMOrder();
			foreach (string sid in ids)
			{
				int.TryParse(sid, out int eid);
				if (eid == 0)
					continue;

				var Event = db.GetEvent(eid).FirstOrDefault();
				if (Event == null)
					continue;

				EMOrderItem item = new EMOrderItem();
				item.EventID = Event.EventID;
				item.EventName = Event.Title;
				item.PriceID = 0;
				item.UnitPrice = 0;
				item.TicketTypeID = 0;
				item.TotalPrice = 0;
				item.UnitCount = 1;
				item.MaxQuota = Event.MaxQuota.Value;
				item.AvailableQuota = item.MaxQuota;
				CartItems.Add(item);

			}

			//EventPrices = db.GetEventPrice(Event.EventID);
			TicketType = new SelectList(db.GetTicketType(), "TicketTypeID", "TypeName");
			

			return Page();
		}

		public ActionResult OnPost(string hchange)
		{
			EMService db = new EMService(config.GetConnectionString("emdb"));
			TicketType = new SelectList(db.GetTicketType(), "TicketTypeID", "TypeName");
			Order.TotalAmount = 0;
			foreach (var item in CartItems)
			{
				var price= db.GetTicketPrice(item.TicketTypeID, item.EventID);
				item.UnitPrice = price.Price;
				item.PriceID=price.PriceID;
				item.TotalPrice = (item.UnitPrice ?? 0) * (item.UnitCount ?? 0);
				item.AvailableQuota = item.AvailableQuota - item.UnitCount ?? 0;
				Order.TotalAmount += item.TotalPrice;
				

			}

			if (hchange == "0")
			{
				Order.UserID = HttpContext.Session.GetInt32("UserID") ?? 0;
				Order.OrderDate = DateTime.Now;
				db.SaveOrder(Order);
				foreach (var item in CartItems)
				{
					item.OrderID = Order.OrderID;
					db.SaveOrderItem(item);
				}
			}
			return Page();
			
		}
		


		

	}
}
