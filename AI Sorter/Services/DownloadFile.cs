using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.JSInterop;
namespace AI_Sorter.Services
{
    public class DownloadFile
    {
        private async Task DownloadFileFromURL()
        {
            var fileName = "quote.txt";
            var fileURL = "/files/quote.txt";
            await JS.InvokeVoidAsync("triggerFileDownload", fileName, fileURL);
        }
    }
}