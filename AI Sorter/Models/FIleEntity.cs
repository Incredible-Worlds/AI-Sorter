namespace AI_Sorter.Models
{
    public class FileEntity
    {
		public FileEntity(int id, string file_name, string promt_sort, string path_file, string path_file_itg, string status_sort)
		{
			this.id = id;
			this.file_name = file_name;
			this.promt_sort = promt_sort;
			this.path_file = path_file;
			this.path_file_itg = path_file_itg;
			Status_sort = status_sort;
		}

		public int id { get; set; } 
        public string file_name { get; set; } // File name
        public string promt_sort { get; set; } // Prompt sort
        public string path_file { get; set; } // Path to the file
        public string path_file_itg { get; set; } // ITG path
        public string Status_sort { get; set; } // Status sort
    }
}
