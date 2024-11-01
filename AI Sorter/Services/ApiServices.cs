using Microsoft.AspNetCore.Components.Forms;
using static System.Net.WebRequestMethods;

namespace AI_Sorter.Services
{
	public class ApiServices
	{
		private static readonly HttpClient client = new HttpClient();
		private const string apiUrl = "http://37.193.240.201:11445/api/Sorter/UploadTable";

		public async Task HandleFileSelected(IBrowserFile File, string prompt)
		{
			try
			{
				var selectedFile = File;

				if (selectedFile != null && prompt != null)
				{
					var content = new MultipartFormDataContent();
					var fileContent = new StreamContent(selectedFile.OpenReadStream(maxAllowedSize: 10485760)); // Макс. размер файла 10 Мб
					fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(selectedFile.ContentType);

					content.Add(fileContent, "file", selectedFile.Name);
					content.Add(new StringContent(prompt), "prompt");

					var response = await client.PostAsync(apiUrl, content);

					if (response.IsSuccessStatusCode)
					{
						Console.WriteLine("Файл успешно загружен!");
					}
					else
					{
						Console.WriteLine("Ошибка загрузки файла.");
					}
				}
			}
			catch (Exception ex)
			{
			}
		}
	}
}
