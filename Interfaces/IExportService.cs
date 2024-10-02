using ImportExportFiles.Models;

namespace ImportExportFiles.Interfaces;

public interface IExportService
{
    public byte[] ExportDataAsExcelFile(string sheetName, IEnumerable<ProductViewModel> data);
}
