using System.ComponentModel.DataAnnotations;

namespace AI_Sorter_Backend.Models
{
	public class Users
	{
		public int id { get; set; }

		[Required]
		public string login { get; set; } = string.Empty;

		[Required]
		public string passwordHash { get; set; } = string.Empty;

		[Required]
		public string role { get; set; } = string.Empty;
	}
}
