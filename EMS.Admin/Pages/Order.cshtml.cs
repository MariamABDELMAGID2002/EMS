using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Admin.Pages
{
    public class OrderModel : PageModel
    {
		private readonly IConfiguration config;
		[BindProperty(SupportsGet = true)]
		public string Act { get; set; } = "";
		[BindProperty]
		public EMOrder Order { get; set; }
		public List<EMOrder> Orders { get; set; }
		public List<EMOrderItem> OrderItems { get; set; }
		public IFormFile upFile { get; set; }
		public OrderModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public void OnGet()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			Orders = db.GetOrder();
			int.TryParse(Act, out int id);
			if (id == 0)
				Order = new EMOrder();
			else
			{
				Order = db.GetOrder(id).FirstOrDefault() ?? new EMOrder();
				OrderItems = db.GetOrderItem(id);
			}

		}
	
		public ActionResult OnPostDelete()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			int.TryParse(Act, out int id);
			db.DeleteOrder(id);
			Orders = db.GetOrder();
			return RedirectToPage("Order", new
			{
				Act = (int?)null,
			});
		}
	}
}
