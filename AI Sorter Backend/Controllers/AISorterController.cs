using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using AI_Sorter_Backend.Models;
using static AI_Sorter_Backend.Models.DbContex;
using AI_Sorter_Backend.Services;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace AI_Sorter_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string ollamaApiUrl = "http://ai-sortme.local:11434";

        public ProxyController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        [HttpPost("generate")]
        public async Task<IActionResult> ProxyGenerate([FromBody] object data)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, ollamaApiUrl + "/api/generate")
            {
                Content = new StringContent(System.Text.Json.JsonSerializer.Serialize(data), Encoding.UTF8, "application/json")
            };

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();
                return Content(responseBody, "application/json");
            }
            catch (HttpRequestException httpEx)
            {
                return StatusCode(500, $"HTTP îøèáêà: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Îøèáêà: {ex.Message}");
            }
        }

        [HttpGet("serviceStatus")]
        public async Task<string> ProxyStatus()
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(ollamaApiUrl);

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync();
                }
                else
                {
                    return $"Îøèáêà: {response.StatusCode}, {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return $"Èñêëþ÷åíèå: {ex.Message}";
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SorterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        public SorterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [DisableCors]        
        [HttpPost("UploadTable")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file, [FromForm] string prompt)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "No file!" });
            }

            // Cheack for type of file
            var allowedExtensions = new[] { ".xls", ".xlsx" };
            var allowedMimeTypes = new[]
            {
                "application/vnd.ms-excel",
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
            };

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Invalid type of file extension. Needed .xls or .xlsx" });
            }

            if (!allowedMimeTypes.Contains(file.ContentType))
            {
                return BadRequest(new { message = "Invalid MIME type" });
            }

            // Write file to filesystem
            var uploadsFolderPath = Path.Combine(Directory.GetCurrentDirectory(), "UploadedFiles");

            if (!Directory.Exists(uploadsFolderPath))
            {
                Directory.CreateDirectory(uploadsFolderPath);
            }

            var uniqueFileName = $"{DateTime.Now.Ticks}{Path.GetExtension(file.FileName)}";

            var filePath = Path.Combine(uploadsFolderPath, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            await AnyPromptSortService.SortDatasheet(filePath, prompt);

            FIleEntity fileEntityDB = new FIleEntity(0, file.FileName, uniqueFileName, filePath, filePath, "process");

            _context.BlazorApp.Add(fileEntityDB);
			await _context.SaveChangesAsync();

			return Ok(new { message = "File loaded!", newFileName = uniqueFileName });
        }
    }

    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FilesController(ApplicationDbContext context)
        {
            _context = context;
        }
        [DisableCors]
        [HttpGet]
        public async Task<IEnumerable<FIleEntity>> GetFiles()
        {
            return _context.BlazorApp.ToList();
        }
        [HttpGet("download/{fileName}")]
		public IActionResult DownloadFile(string fileName)
		{
			var filePath = Path.Combine("UploadedFiles", fileName); // Путь к файлу

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound();
			}

			var fileBytes = System.IO.File.ReadAllBytes(filePath);
			return File(fileBytes, "application/octet-stream", fileName);
		}
	}

	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly ApplicationDbContext _context;
		private readonly IConfiguration _config;

		public AuthController(ApplicationDbContext context, IConfiguration config)
		{
			_context = context;
			_config = config;
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == request.Login);
			if (user == null)
				return Unauthorized();

			var hasher = new PasswordHasher<User>();
			var result = hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);

			if (result == PasswordVerificationResult.Failed)
				return Unauthorized();

			var token = GenerateJwtToken(user);
			return Ok(new { token });
		}

		private string GenerateJwtToken(User user)
		{
			var claims = new[]
			{
			new Claim(ClaimTypes.Name, user.Login),
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
		};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
			var expires = DateTime.UtcNow.AddHours(3);

			var token = new JwtSecurityToken(
				_config["Jwt:Issuer"],
				_config["Jwt:Issuer"],
				claims,
				expires: expires,
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}

}
