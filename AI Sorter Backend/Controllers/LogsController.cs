using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AI_Sorter_Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class LogsController : ControllerBase
	{
		private readonly string logDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Logs");

		[HttpGet("getlogs")]
		public IActionResult GetLogs([FromQuery] int lines = 200)
		{
			if (!Directory.Exists(logDirectory))
				return NotFound(new { message = "Log directory not found." });

			var logFiles = Directory.GetFiles(logDirectory, "*.txt", SearchOption.TopDirectoryOnly);
			if (!logFiles.Any())
				return NotFound(new { message = "No log files found." });

			var latestLog = logFiles
				.Select(f => new FileInfo(f))
				.OrderByDescending(f => f.CreationTime)
				.First();

			var allLines = System.IO.File.ReadLines(latestLog.FullName)
				.Reverse()
				.Take(lines)
				.Reverse();

			return Ok(new { file = latestLog.Name, content = string.Join("\n", allLines) });
		}
	}
}
