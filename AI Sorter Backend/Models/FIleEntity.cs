namespace AI_Sorter_Backend.Models
{
    public class FIleEntity(int id, string file_name, string unic_file_name, string path_file, string path_file_competed, string status_sort)
	{
		public int id { get; set; } = id;
		public string file_name { get; set; } = file_name;
		public string unic_file_name { get; set; } = unic_file_name;
		public string path_file { get; set; } = path_file;
		public string path_file_competed { get; set; } = path_file_competed;
		public string Status_sort { get; set; } = status_sort;

		public DateTime datetime { get; set; } = DateTime.UtcNow;
	}
}
