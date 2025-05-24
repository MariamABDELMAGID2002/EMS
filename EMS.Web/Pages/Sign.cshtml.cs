using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Web.Pages
{
    public class SignModel : PageModel
    {
		private readonly IConfiguration config;
		public SignModel(IConfiguration _configuration)
		{
			config = _configuration;
		}
		public string Msg { get; set; } = "";
		public void OnGet()
		{
			HttpContext.Session.SetString("UserName", "");
		}
		public ActionResult OnPost(string email,string password)
		{
			EMService db = new EMService(config.GetConnectionString("emdb"));
			
			if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
			{
				Msg = "Please enter email and password";
				return Page();
			}
			UMSUser user = db.Login(email, password);
			if (user is null)
				Msg = "Invalid email or password";
			else if (!user.Approved)
				Msg = "User not Approved yet!";

			if (Msg == "")
			{
				HttpContext.Session.SetString("UserName", user.RealName);
				HttpContext.Session.SetInt32("UserID", user.UserID);
				if(user.PassExpired)
					return RedirectToPage("/Reset");
				else
					return RedirectToPage("/Index");
			}
			else
				return Page();

			return Page();
		}
		public ActionResult OnPostReg(string name,string email, string password,string confirm)
		{
			EMService db = new EMService(config.GetConnectionString("emdb"));

			if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirm))
			{
				Msg = "Please fill all data";
				return Page();
			}
			if (password != confirm)
			{
				Msg = "Password and confirm password not matched";
				return Page();
			}
			UMSUser user = new UMSUser();
			user.RealName = name;
			user.Email = email;
			user.Password = password;
			user.PassExpired = true;
			user.Approved = false;
			user.RegisterDate = DateTime.Now;
			user.IsAdmin = false;
			db.SaveUser(user);

			ViewData["Msg"] = "Thank you, Registred Successfully.";

			return RedirectToPage("Sign", new
			{
				handler = (int?)null,
			});
		}

	}
}
