using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Web.Pages
{
    public class ResetModel : PageModel
    {
		private readonly IConfiguration config;
		public ResetModel(IConfiguration _configuration)
		{
			config = _configuration;
		}
		public string Msg { get; set; } = "";
		public void OnGet()
		{
			
		}
		public ActionResult OnPost(string newpass,string confirm)
		{
			EMService db = new EMService(config.GetConnectionString("emdb"));
			int userid = HttpContext.Session.GetInt32("UserID") ?? 0;
			if (userid == 0)
			{
				return RedirectToPage("/Sign");
			}
			if ( string.IsNullOrEmpty(newpass) || string.IsNullOrEmpty(confirm))
			{
				Msg = "Please fill all data";
				return Page();
			}
			if (confirm != newpass)
			{
				Msg = "New password and confirm not matched";
				return Page();
			}
			UMSUser user = db.GetUser(userid).FirstOrDefault()??new UMSUser();
			
			user.Password = newpass;
			user.PassExpired = false;
			db.SaveUser(user);

			return RedirectToPage("/Sign");
			


		}
	}
}
