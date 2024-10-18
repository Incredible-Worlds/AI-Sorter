using Newtonsoft.Json;
using OfficeOpenXml;
using System.Text;
using System.Diagnostics;

namespace AI_Sorter_Backend.Services;

public class AnyPromptSortService
{
    public static async Task SortDatasheet(string filePath, string prompt)
    {
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0]; 
            var startRow = 1;
            var endRow = worksheet.Dimension.End.Row;
            var columnToSort = 1;

            // Читаем данные из диапазона
            var data = worksheet.Cells[startRow, 1, endRow, worksheet.Dimension.End.Column]
                .Select(cell => new
                {
                    Row = cell.Start.Row,
                    Col = cell.Start.Column,
                    Value = worksheet.Cells[cell.Start.Row, cell.Start.Column].Value
                })
                .GroupBy(c => c.Row)
                .Select(g => g.Select(c => new { c.Value }).ToList())
                .ToList();

            var sortedData = data.OrderBy(row => row[columnToSort - 1].Value).ToList();

            worksheet.Cells[startRow, 1, endRow, worksheet.Dimension.End.Column].Clear();

            for (int i = 0; i < sortedData.Count; i++)
            {
                for (int j = 0; j < sortedData[i].Count; j++)
                {
                    worksheet.Cells[startRow + i, j + 1].Value = sortedData[i][j].Value;
                }
            }

            Task.Run(() => DeletePromptData(prompt));

            package.Save(); 
        }
    }

    private static async Task DeletePromptData(string _prompt)
    {
        HttpClient aspClient = new HttpClient();
        const string apiUrl = "http://localhost:11434/api/generate";

        string sortPrompt = "Запомни следующую информацию: я буду предоставлять тебе список данных," +
            " а ты должен без лишних символов (без markdown) выводить лишние элементы," +
            " не попадающие под тему сортировки с новой строки в том виде, как я тебе их передал." +
            $" Тема сортировки: \"{_prompt}\"." +
            " Если предмет попадает под тему сортировки, то ты должен ответить \"OK\" без лишних символов (без markdown)." +
            " Пример запроса: \"Амортизатор пружинно-гидравлический задний LX450-F15-1, F15-1, LX450-2210000" +
            "\r\nАмортизатор пружинно-гидравлический задний HN110GY-5C-050000" +
            "\r\nАксесуар-игрушка утка 2151-5125125-125125" +
            "\r\nАмортизатор пружинно-гидравлический задний 2915000-210-0000 \"" +
            " Пример ответа: \"OK \r\nOK \r\nАксесуар-игрушка утка 2151-5125125-125125 \r\nOK\"" +
            " Если всё понятно, то ответь \"OK\"";

        var requestData = new
        {
            model = "gemma2",
            stream = false,
            prompt = sortPrompt
        };

        var jsonContent = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await aspClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);
            if (jsonResponse.response == "OK")
            {
                Debug.WriteLine("200 OK");
            }
        }
    }
}
