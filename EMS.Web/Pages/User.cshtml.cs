using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Web.Pages
{
    public class UserModel : PageModel
    {
		private readonly IConfiguration config;
		public UserModel(IConfiguration _config)
		{
			config = _config;
		}
		public List<EMEventType> EventTypes { get; set; }
		public List<UMInterest> UserInterest { get; set; }
		public void OnGet()
        {
			int userid = HttpContext.Session.GetInt32("UserID") ?? 0;
			if (userid == 0)
				RedirectToPage("/Sign");

			var db = new EMService(config.GetConnectionString("emdb"));
			EventTypes = db.GetEventType();
			UserInterest = db.GetInterest(userid);
			
		}

		public ActionResult OnPost(string chk)
		{
			int userid = HttpContext.Session.GetInt32("UserID") ?? 0;
			if (userid == 0)
				return RedirectToPage("/Sign");
			var db = new EMService(config.GetConnectionString("emdb"));

			if (string.IsNullOrEmpty(chk))
			{
				return Page();
			}
			string[] ids = chk.Split(',');
			db.DeleteInterest(userid);
			foreach (string id in ids)
			{
				if (!int.TryParse(id, out int temp))
					continue;
				UMInterest obj = new UMInterest();
				obj.UserID = userid;
				obj.EventType = int.Parse(id);
				db.SaveInterest(obj);
			}
			
			EventTypes = db.GetEventType();
			UserInterest = db.GetInterest(userid);
			return Page();
		}
    }
}
