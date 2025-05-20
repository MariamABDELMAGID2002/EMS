using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Admin.Pages
{
    public class AnnounceModel : PageModel
    {
		private readonly IConfiguration config;
		public List<EMAnnounce> Announcments { get; set; }
		public AnnounceModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public void OnGet()
        {
			EMService db = new EMService(config.GetConnectionString("emdb"));
			Announcments = db.AnnounceGet();
		}
		public void OnPost()
		{

			EMAnnounce objAnnounce = new EMAnnounce();
			objAnnounce.Title = "Test Title";
			objAnnounce.Description = "Test Description";
			objAnnounce.CreatedAt = DateTime.Now;
			objAnnounce.UpdatedAt = null;
			objAnnounce.Published = true;
			EMService db = new EMService(config.GetConnectionString("emdb"));
			db.AnnounceSave(objAnnounce);
			Announcments = db.AnnounceGet();
		}
	}
}
