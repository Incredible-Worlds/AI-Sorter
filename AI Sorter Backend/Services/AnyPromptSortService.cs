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

            Task.Run(() => DeletePromptData(filePath, prompt));

            package.Save(); 
        }
    }

    private static async Task DeletePromptData(string filePath, string _prompt)
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
                AISortDatasheet(filePath);
            }
        }
    }

    private static async void AISortDatasheet(string filePath)
    {
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var startRow = 1;
            var endRow = worksheet.Dimension.End.Row;
            var columnToSort = 1;
            int batchSize = 5; // Размер группы

            // Читаем данные из диапазона
            var data = worksheet.Cells[startRow, 1, endRow, worksheet.Dimension.End.Column]
                .Select(cell => new
                {
                    Row = cell.Start.Row,
                    Col = cell.Start.Column,
                    Value = worksheet.Cells[cell.Start.Row, cell.Start.Column].Value
                })
                .GroupBy(c => c.Row)
                .Select(g => g.Select(c => new { c.Value, g.Key }).ToList())
                .ToList();

            // Разбиваем данные на батчи по 10 записей
            for (int i = 0; i < data.Count; i += batchSize)
            {
                var batch = data.Skip(i).Take(batchSize).ToList();
                var batchValues = string.Join("\r\n", batch.Select(row => row[0].Value.ToString()));

                // Отправляем батч в API
                var result = await SendDataToAPI(batchValues);

                // Обрабатываем ответ и меняем цвет ячеек
                for (int j = 0; j < batch.Count; j++)
                {
                    if (result[j] == "OK") // Если ответ "OK"
                    {
                        // Зеленый цвет для ячейки
                        worksheet.Cells[batch[j][0].Key, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[batch[j][0].Key, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                    }
                    else
                    {
                        // Красный цвет для ячейки
                        worksheet.Cells[batch[j][0].Key, 1].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                        worksheet.Cells[batch[j][0].Key, 1].Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
                    }
                }
            }

            package.Save();
        }
    }

    private static async Task<List<string>> SendDataToAPI(string batchValues)
    {
        HttpClient aspClient = new HttpClient();
        const string apiUrl = "http://localhost:11434/api/generate";

        var requestData = new
        {
            model = "gemma2",
            stream = false,
            prompt = batchValues
        };

        var jsonContent = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await aspClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);

            // Предполагается, что API вернёт список строк с результатами для каждой записи
            return ((IEnumerable<dynamic>)jsonResponse.results).Select(r => (string)r).ToList();
        }

        return new List<string>(); // Пустой список, если запрос неудачен
    }
}
