using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using EMS.Data;
using System.Runtime.Versioning;
namespace EMS.Admin.Pages
{
    public class LoginModel : PageModel
    {
        IConfiguration config;
        public LoginModel(IConfiguration _config)
        {
            config = _config;
        }
        public string ErrMsg { get; set; } = "";
        public void OnGet()
        {
			HttpContext.Session.SetString("UserName", "");
		}

        public IActionResult OnPost(string username, string password)
        {
			if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ErrMsg = "Please enter email and password";
				return Page();
			}
            EMService db = new EMService(config.GetConnectionString("emdb"));
            UMSUser user =db.UserLogin(username, password.Trim());
            if (user is null)
                ErrMsg = "Invalid email or password";
            else if (!user.IsAdmin.Value)
                ErrMsg = "This user is not admin";
            else if (!user.Approved.Value)
                ErrMsg = "User not Approved yet!";

            if (ErrMsg == "")
            {
				HttpContext.Session.SetString("UserName", user.RealName);
				return RedirectToPage("/Index");
            }
            else
                return Page();
		}

    }
}
