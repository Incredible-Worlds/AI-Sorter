namespace AI_Sorter_Backend.Models
{
	public class User
	{
		public int Id { get; set; }
		public string Login { get; set; } = string.Empty;
		public string PasswordHash { get; set; } = string.Empty;
	}
}
