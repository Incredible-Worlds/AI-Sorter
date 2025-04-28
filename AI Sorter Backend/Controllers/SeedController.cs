using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using AI_Sorter_Backend.Models;
using static AI_Sorter_Backend.Models.DbContex;
using Microsoft.EntityFrameworkCore;
using AI_Sorter_Backend.Models;

namespace AI_Sorter_Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public partial class SeedController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		public SeedController(ApplicationDbContext db)
		{
			_db = db;
		}

		[HttpPost("seeding")]
		public async Task<IActionResult> SeedUser([FromBody] SeedUserDto dto)
		{
			if (string.IsNullOrWhiteSpace(dto.login) || string.IsNullOrWhiteSpace(dto.password))
				return BadRequest("Логин и пароль обязательны");

			var existing = await _db.Users.FirstOrDefaultAsync(u => u.login == dto.login);
			if (existing != null)
				return BadRequest("Пользователь уже существует");

			var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.password);

			var user = new Users
			{
				login = dto.login,
				passwordHash = passwordHash,
				role = string.IsNullOrEmpty(dto.role) ? "user" : dto.role  // ← роль по умолчанию "user"
			};

			_db.Users.Add(user);
			await _db.SaveChangesAsync();

			return Ok("Пользователь добавлен");
		}
	}
}
