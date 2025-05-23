using EMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace EMS.Admin.Pages
{
    public class AnnounceModel : PageModel
    {
		private readonly IConfiguration config;
		[BindProperty(SupportsGet =true)]
		public string Act { get; set; } = "";
		[BindProperty]
		public EMAnnounce Announce { get; set; }
		public List<EMAnnounce> Announcments { get; set; }
		public IFormFile upFile { get; set; }
		public AnnounceModel(IConfiguration _configuration)
		{
			config = _configuration;
		}

		public void OnGet()
        {
			
			EMService db = new EMService(config.GetConnectionString("emdb"));
			Announcments = db.GetAnnounce();
			int.TryParse(Act, out int id);
			if (id == 0)
				Announce = new EMAnnounce();
			else 
				Announce = db.GetAnnounce(id).FirstOrDefault()??new EMAnnounce();

		}
		public ActionResult OnPost()
		{

			EMService db = new EMService(config.GetConnectionString("emdb"));
			if(Announce.AnnounceID!=0)
				Announce.UpdatedAt = DateTime.Now;
			db.SaveAnnounce(Announce);

			if (upFile != null && upFile.Length > 0)
			{
				var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "upload", "a"+Announce.AnnounceID+".jpg");
				using (var fileStream = new FileStream(filePath, FileMode.Create))
				{
					 upFile.CopyTo(fileStream);
				}
			}


			Announcments = db.GetAnnounce();
			return RedirectToPage("Announce", new
			{
				Act = (int?)null,
			});
		}
		public ActionResult OnPostDelete()
		{
			
			EMService db = new EMService(config.GetConnectionString("emdb"));
			int.TryParse(Act, out int id);
			db.DeleteAnnounce(id);
			Announcments = db.GetAnnounce();
			return RedirectToPage("Announce", new
			{
				Act = (int?)null,
			});
		}
		
	}
}
