using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Admin.Pages
{
    public class UserModel : PageModel
    {
		private readonly IConfiguration config;
		[BindProperty(SupportsGet = true)]
		public string Act { get; set; } = "";
		[BindProperty]
		public UMSUser User { get; set; }
		public List<UMSUser> Users { get; set; }
		public string Err { get; set; } = "";
		[BindProperty]
		public string NewPassword { get; set; } = "";
		public UserModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public void OnGet()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			Users = db.GetUser();
			int.TryParse(Act, out int id);
			if (id == 0)
				User = new UMSUser();
			else
				User = db.GetUser(id).FirstOrDefault() ?? new UMSUser();

		}
		public ActionResult OnPost()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			if(!string.IsNullOrEmpty(NewPassword))
				User.Password = NewPassword;
			db.SaveUser(User);
			Users = db.GetUser();
			return RedirectToPage("User", new
			{
				Act = (int?)null,
			});
		}
		public ActionResult OnPostDelete()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			Users = db.GetUser();
			int.TryParse(Act, out int id);

			int usedCount = db.GetUsedCount(EMService.UsedTypes.users, id);
			if (usedCount > 0)
			{
				Err = $"Can't delete this user because it used {usedCount} times";
				return Page();
			}
			if (db.IsAdmin(id))
			{
				Err = $"admin users can't be deleted";
				return Page();
			}
			db.DeleteUser(id);
			return RedirectToPage("User", new
			{
				Act = (int?)null,
			});
		}
	}
}
