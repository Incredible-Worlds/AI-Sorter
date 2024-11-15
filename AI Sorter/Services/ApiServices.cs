using Microsoft.AspNetCore.Components.Forms;
using static System.Net.WebRequestMethods;
using AI_Sorter.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Newtonsoft.Json;
using System.Net.Http.Json;

namespace AI_Sorter.Services
{
	public class ApiServices
	{
		private static readonly HttpClient client = new HttpClient();
		private const string apiUrl = "http://localhost:11445/api/";

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

					var response = await client.PostAsync(apiUrl + "Sorter/UploadTable", content);

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

		public static async Task<List<FileEntity>>UploadFile () 
		{
			List<FileEntity> fileEntity = new List<FileEntity>();

			try
			{
				fileEntity = await client.GetFromJsonAsync<List<FileEntity>>(apiUrl + "Files/");
			}
			catch (Exception ex)
			{

			}

			return fileEntity;
		}


	}
}
