using AI_Sorter_Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static AI_Sorter_Backend.Models.DbContex;

namespace AI_Sorter_Backend.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly ApplicationDbContext _context;

		public UsersController(ApplicationDbContext context)
		{
			_context = context;
		}

		// Получение всех пользователей
		[HttpGet("get-all")]
		public async Task<ActionResult<List<User>>> GetAllUsers()
		{
			var users = await _context.Users
									   .Select(u => new User { Login = u.login, Role = u.role })
									   .ToListAsync();
			return Ok(users);
		}

		// Удаление пользователя по логину
		[HttpDelete("delete/{login}")]
		public async Task<IActionResult> DeleteUser(string login)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.login == login);

			if (user == null)
			{
				return NotFound(new { message = "Пользователь не найден" });
			}

			_context.Users.Remove(user);
			await _context.SaveChangesAsync();

			return NoContent();
		}
	}
}
