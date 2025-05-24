using EMS.Data;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Admin.Controllers
{
	[Route("[controller]/[Action]")]
	[ApiController]
	public class ApiController : Controller
	{
		private readonly IConfiguration config;
		public ApiController(IConfiguration _configuration)
		{
			config = _configuration;
		}


		[HttpGet]
		public IActionResult GetEvents(int eventid = 0, int userid = 0)
		{
			EMService db = new EMService(config.GetConnectionString("emdb"));
			var events = db.GetEvent(eventid);
			return Ok(events);
		}
	}
}
