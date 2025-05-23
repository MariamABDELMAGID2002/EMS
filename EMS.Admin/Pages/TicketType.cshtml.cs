using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Admin.Pages
{
    public class TicketTypeModel : PageModel
    {
		private readonly IConfiguration config;
		[BindProperty(SupportsGet = true)]
		public string Act { get; set; } = "";
		[BindProperty]
		public EMTicketType TicketType { get; set; }
		public List<EMTicketType> TicketTypes { get; set; }
		public string Err { get; set; } = "";
		public TicketTypeModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public void OnGet()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			TicketTypes = db.GetTicketType();
			int.TryParse(Act, out int id);
			if (id == 0)
				TicketType = new EMTicketType();
			else
				TicketType = db.GetTicketType(id).FirstOrDefault() ?? new EMTicketType();

		}
		public ActionResult OnPost()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			db.SaveTicketType(TicketType);
			TicketTypes = db.GetTicketType();
			return RedirectToPage("TicketType", new
			{
				Act = (int?)null,
			});
		}
		public ActionResult OnPostDelete()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			TicketTypes = db.GetTicketType();
			int.TryParse(Act, out int id);

			int usedCount = db.GetUsedCount(EMService.UsedTypes.ticketTypes, id);
			if (usedCount > 0)
			{
				Err = $"Can't delete this type because it used {usedCount} times";
				return Page();
			}
			db.DeleteTicketType(id);
			return RedirectToPage("TicketType", new
			{
				Act = (int?)null,
			});
		}
	}
}
