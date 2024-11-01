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

            package.Save();

            Task.Run(() => AISortDatasheet(filePath, prompt));
        }
    }

    private static async void AISortDatasheet(string filePath, string prompt)
    {
        using (var package = new ExcelPackage(new FileInfo(filePath)))
        {
            var worksheet = package.Workbook.Worksheets[0];
            var startRow = 1;
            var endRow = worksheet.Dimension.End.Row;
            var columnToSort = 1;
            int batchSize = 10; // Размер группы

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
                var result = await SendDataToAPI(batchValues, prompt);

                // Обрабатываем ответ и меняем цвет ячеек

                for (int j = 0; j < result.Count; j++)
                {
                    for (int k = 0; k < batch.Count; k++)
                    {
                        // Получаем значение ячейки
                        var cell = worksheet.Cells[batch[k][0].Key, 1];
                        var celldata = cell.Value.ToString();

                        // Проверяем, если ячейка уже окрашена в красный (LightCoral), пропускаем её
                        var currentColor = cell.Style.Fill.BackgroundColor.Rgb;
                        if (currentColor != null && currentColor.Equals("FFF08080")) // RGB код для LightCoral
                        {
                            continue; // Пропускаем ячейки, которые уже были окрашены в красный
                        }

                        // Если данные совпадают с результатом, окрашиваем в красный
                        if (celldata == result[j])
                        {
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightCoral);
                        }
                        else
                        {
                            // В противном случае окрашиваем в зелёный
                            cell.Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
                            cell.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.LightGreen);
                        }
                    }
                }
            }   

            package.Save();
        }
    }

    private static async Task<List<string>> SendDataToAPI(string batchValues, string sortingPrompt)
    {
        HttpClient aspClient = new HttpClient();
        const string apiUrl = "http://localhost:11434/api/generate";

        var requestData = new
        {
            model = "gemma2",
            stream = false,
            prompt = $"Выведи ниже всё, что не относится к теме \"{sortingPrompt}\":" + batchValues + " \nЕсли что-то относится к этой теме, то не указывай этот элемент ни в коем случае в ответе. Отвечай строго выводом введённых данных без лишних слов. Разделитель записей \";\""
        };

        var jsonContent = JsonConvert.SerializeObject(requestData);
        var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await aspClient.PostAsync(apiUrl, content);

        if (response.IsSuccessStatusCode)
        {
            // Чтение ответа как строки
            string responseBody = await response.Content.ReadAsStringAsync();

            // Десериализация JSON
            var jsonResponse = JsonConvert.DeserializeObject<dynamic>(responseBody);

            // Извлечение значения из поля "response"
            string responseText = jsonResponse.response.ToString();

            // Разделяем текст на строки по символам новой строки или точке с запятой
            var result = responseText.Split(new[] { "\r\n", "\r", "\n", ";" }, StringSplitOptions.None);

            return result.ToList();
        }

        return new List<string>(); // Пустой список, если запрос неудачен
    }
}
