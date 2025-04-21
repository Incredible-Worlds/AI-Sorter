using AI_Sorter_Backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace AI_Sorter_Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SystemController : ControllerBase
	{
		private readonly SystemInfoService _systemInfoService;

		public SystemController(SystemInfoService systemInfoService)
		{
			_systemInfoService = systemInfoService;
		}

		[HttpGet("load")]
		public IActionResult GetSystemLoad()
		{
			var data = _systemInfoService.GetSystemLoad();
			return Ok(data);
		}
	}

}
