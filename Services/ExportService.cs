using ClosedXML.Excel;
using ImportExportFiles.Constants;
using ImportExportFiles.Interfaces;
using ImportExportFiles.Models;

namespace ImportExportFiles.Services;

public class ExportService : IExportService
{
    public byte[] ExportDataAsExcelFile(string sheetName, IEnumerable<ProductViewModel> data)
    {
        // Create a new Excel workbook using ClosedXML
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add(sheetName);

        // Create header row
        worksheet.Cell(1, 1).Value = HeaderNames.BandNumber;
        worksheet.Cell(1, 2).Value = HeaderNames.CategoryCode;
        worksheet.Cell(1, 3).Value = HeaderNames.Manufacturer;
        worksheet.Cell(1, 4).Value = HeaderNames.PartSku;
        worksheet.Cell(1, 5).Value = HeaderNames.ItemDescription;
        worksheet.Cell(1, 6).Value = HeaderNames.ListPrice;
        worksheet.Cell(1, 7).Value = HeaderNames.MinDiscount;
        worksheet.Cell(1, 8).Value = HeaderNames.DiscountPrice;

        // Insert the product data
        int row = 2;
        foreach (var item in data)
        {
            worksheet.Cell(row, 1).Value = item.BandNumber;
            worksheet.Cell(row, 2).Value = item.CategoryCode;
            worksheet.Cell(row, 3).Value = item.Manufacturer;
            worksheet.Cell(row, 4).Value = item.PartSku;
            worksheet.Cell(row, 5).Value = item.ItemDescription;
            worksheet.Cell(row, 6).Value = item.ListPrice;
            worksheet.Cell(row, 7).Value = item.MinDiscount;
            worksheet.Cell(row, 8).Value = item.DiscountPrice;
            row++;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}

