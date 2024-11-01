namespace AI_Sorter_Backend.Models
{
    public class FIleEntity(int id, string file_name, string promt_sort, string path_file, string path_file_itg, string status_sort)
	{
		public int id { get; set; } = id;
		public string file_name { get; set; } = file_name;
		public string promt_sort { get; set; } = promt_sort;
		public string path_file { get; set; } = path_file;
		public string path_file_itg { get; set; } = path_file_itg;
		public string Status_sort { get; set; } = status_sort;
	}
}
