namespace AI_Sorter_Backend.Models
{
    public class FIleEntity
    {
        public int id { get; set; } 
        public string file_name { get; set; } // File name
        public string promt_sort { get; set; } // Prompt sort
        public string path_file { get; set; } // Path to the file
        public string path_file_itg { get; set; } // ITG path
        public string Status_sort { get; set; } // Status sort
    }
}
