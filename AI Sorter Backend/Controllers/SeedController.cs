using Microsoft.AspNetCore.Mvc;
using BCrypt.Net;
using AI_Sorter_Backend.Models;
using static AI_Sorter_Backend.Models.DbContex;
using Microsoft.EntityFrameworkCore;

namespace AI_Sorter_Backend.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class SeedController : ControllerBase
	{
		private readonly ApplicationDbContext _db;
		public SeedController(ApplicationDbContext db)
		{
			_db = db;
		}

		[HttpPost("seeding")]
		public async Task<IActionResult> SeedUser()
		{
			var existing = await _db.Users.FirstOrDefaultAsync(u => u.login == "admin");
			if (existing != null)
				return BadRequest("Пользователь уже существует");

			var password = "admin";
			var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

			var user = new Users
			{
				login = "admin", // свойство должно совпадать с моделью
				passwordHash = passwordHash
			};

			_db.Users.Add(user); // с большой буквы
			await _db.SaveChangesAsync();

			return Ok("Пользователь добавлен");
		}
	}
}
