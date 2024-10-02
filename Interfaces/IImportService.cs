namespace ImportExportFiles.Interfaces;

public interface IImportService
{
    Task ImportDataFromExcelFileAsync(IFormFile file);
}
