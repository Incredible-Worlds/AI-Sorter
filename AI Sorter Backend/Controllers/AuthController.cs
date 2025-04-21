using Microsoft.AspNetCore.Mvc;
using System.Text;
using AI_Sorter_Backend.Models;
using static AI_Sorter_Backend.Models.DbContex;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.EntityFrameworkCore;

namespace AI_Sorter_Backend.Controllers
{
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
		public async Task<IActionResult> Login([FromBody] Models.LoginRequest request)
		{
			var user = await _context.Users.FirstOrDefaultAsync(u => u.login == request.Login);
			if (user == null)
				return BadRequest(new { message = "Неизвестный логин" });

			bool isValidPassword = BCrypt.Net.BCrypt.Verify(request.Password, user.passwordHash);
			if (!isValidPassword)
				return BadRequest(new { message = "Неизвестный пароль" });

			var token = GenerateJwtToken(user);
			return Ok(new { token });
		}

		private string GenerateJwtToken(Users user)
		{
			var claims = new[]
			{
				new Claim(ClaimTypes.Name, user.login),
				new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
				new Claim(ClaimTypes.Role, user.role)
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
