using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace AI_Sorter.Services
{
    public class DownloadFile
    {
        public async Task DownloadFil(string filePath, string fileName, HttpContext httpContext)
        {
            try
            {
                byte[] fileBytes = await File.ReadAllBytesAsync(filePath);

                // Setting the headers for downloading the file
                httpContext.Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{fileName}\"");
                httpContext.Response.ContentType = "application/octet-stream"; // Setting the content type

                //We send the file in the response
                await httpContext.Response.Body.WriteAsync(fileBytes);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при скачивании файла: {ex.Message}");
            }
        }
    }
}
