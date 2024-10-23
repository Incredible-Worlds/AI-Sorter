using Microsoft.AspNetCore.Components.Forms;

namespace AI_Sorter.Services
{
	public class UploadFile
	{
		public string FileName { get; set; } = "";
		public long FileSize { get; set; }
		public string FileType { get; set; } = "";
		public DateTimeOffset LastModified { get; set; }
		public string ErrorMessage { get; set; } = "";

		const int MAX_FILESIZE = 5000 * 1024; // 2 MB

		public async Task UploadFiles(InputFileChangeEventArgs e)
		{
			var browserFile = e.File;

			if (browserFile != null) //checking an empty file or not
			{
				FileSize = browserFile.Size;
				FileType = browserFile.ContentType;
				FileName = browserFile.Name; // the name of the uploaded file
				LastModified = browserFile.LastModified; //the date the file was last modified

				try
				{
					var fileStream = browserFile.OpenReadStream(MAX_FILESIZE); //size limit

					var randomFile = Path.GetTempFileName(); //temporary random name
					var extension = Path.GetExtension(browserFile.Name); // file extension
					var targetFilePath = Path.ChangeExtension(randomFile, extension); //The target path of the file with the extension from the original file is created
					var destinationStream = new FileStream(targetFilePath, FileMode.Create);
					await fileStream.CopyToAsync(destinationStream); //The contents of the downloaded file are copied asynchronously to the target file
					destinationStream.Close(); //The write stream to the target file is closed
				}
				catch (Exception exception)
				{
					ErrorMessage = exception.Message;
				}
			}
		}
	}
}


