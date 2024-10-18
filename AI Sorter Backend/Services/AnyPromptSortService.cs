using OfficeOpenXml;

namespace AI_Sorter_Backend.Services;

public class AnyPromptSortService
{
    public static async Task SortDatasheet(string filePath)
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
        }
    }
}
