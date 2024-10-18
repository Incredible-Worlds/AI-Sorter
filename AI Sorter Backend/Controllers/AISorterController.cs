using Microsoft.AspNetCore.Mvc;
using System.Text;
using Microsoft.AspNetCore.Http.HttpResults;
using AI_Sorter_Backend.Models;
using static AI_Sorter_Backend.Models.DbContex;

namespace AI_Sorter_Backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string ollamaApiUrl = "http://localhost:11434";

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
                return StatusCode(500, $"HTTP ошибка: {httpEx.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Ошибка: {ex.Message}");
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
                    return $"Ошибка: {response.StatusCode}, {response.ReasonPhrase}";
                }
            }
            catch (Exception ex)
            {
                return $"Исключение: {ex.Message}";
            }
        }
    }

    [ApiController]
    [Route("api/[controller]")]
    public class SorterController : ControllerBase
    {
        [HttpPost("UploadTable")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("Файл не выбран");
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
                return BadRequest(new { message = "Неверный формат файла. Разрешены только .xls и .xlsx" });
            }

            if (!allowedMimeTypes.Contains(file.ContentType))
            {
                return BadRequest(new { message = "Неверный MIME-тип файла" });
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

            return Ok(new { message = "Файл загружен", newFileName = uniqueFileName });
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

        [HttpPost]
        public async Task<ActionResult<FIleEntity>> CreatePostgresFile([FromBody] FIleEntity postgresFile)
        {
            _context.BlazorApp.Add(postgresFile);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Запись добавлена!", postgresFile });
        }
        [HttpGet]
        public async Task<IEnumerable<FIleEntity>> GetFiles()
        {
            return _context.BlazorApp.ToList();
        }

    }
}
