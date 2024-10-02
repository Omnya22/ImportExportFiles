using ExcelDataReader;
using ImportExportFiles.Constants;
using ImportExportFiles.Interfaces;
using ImportExportFiles.Models;
using System.Data;
using System.Text;

namespace ImportExportFiles.Services;

public class ImportService(
    IProductService productService,
    ILogger<ImportService> logger) : IImportService
{
    private const int BatchSize = 1000;

    public async Task ImportDataFromExcelFileAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            logger.LogError("No file uploaded.");
            throw new ArgumentException("No file uploaded.");
        }

        using var stream = file.OpenReadStream();
        using var reader = ExcelReaderFactory.CreateReader(stream);
        var dataset = reader.AsDataSet(new ExcelDataSetConfiguration
        {
            ConfigureDataTable = (_) => new ExcelDataTableConfiguration
            {
                UseHeaderRow = true
            }
        });

        for (int sheetNum = 0; sheetNum < 3 && sheetNum < dataset.Tables.Count; sheetNum++)
        {
            int rowNo = 0;
            string errBatchMsg = string.Empty;
            var dataTable = dataset.Tables[sheetNum];
            var rows = dataTable.Rows.Cast<DataRow>().AsEnumerable();
            for (int i = 0; i < dataTable.Rows.Count; i += BatchSize)
            {
                var batch = rows.Skip(i).Take(BatchSize);
                (rowNo,errBatchMsg) = await ProcessBatchAsync(batch, rowNo);
            }
            if (!string.IsNullOrWhiteSpace(errBatchMsg))
                logger.LogWarning($"Sheet {sheetNum + 1}: {errBatchMsg}");
            else
                logger.LogInformation($"Sheet {sheetNum + 1}: Batch processed successfully.");


        }
    }

    private async Task<(int,string)> ProcessBatchAsync(IEnumerable<DataRow> batch, int rowNo)
    {
        StringBuilder errBatch = new();
        List<ProductViewModel> products = [];
        foreach (DataRow row in batch)
        {
            try
            {
                products.Add(
                    new ProductViewModel
                    {
                        BandNumber = row[HeaderNames.BandNumber].ToString(),
                        CategoryCode = row[HeaderNames.CategoryCode].ToString(),
                        Manufacturer = row[HeaderNames.Manufacturer].ToString(),
                        PartSku = row[HeaderNames.PartSku].ToString(),
                        ItemDescription = row[HeaderNames.ItemDescription].ToString(),
                        ListPrice = Convert.ToDecimal(row[HeaderNames.ListPrice]),
                        MinDiscount = Convert.ToDecimal(row[HeaderNames.MinDiscount]),
                        DiscountPrice = Convert.ToDecimal(row[HeaderNames.DiscountPrice])
                    });

                var validationErrors = ValidateRow(row, rowNo);
                if (!string.IsNullOrWhiteSpace(validationErrors))
                {
                    errBatch.AppendLine($"Skipping row {rowNo} due to validation errors: {validationErrors}");
                    continue;
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, $"Error processing row {rowNo}: {ex.Message}");
            }
            rowNo++;
        }
        await productService.AddOrUpdateProductsAsync(products);
        return (rowNo,errBatch.ToString());
    }

    private string ValidateRow(DataRow row, int rowNo)
    {
        StringBuilder errMessages = new();
        AppendIfNotEmpty(RequiredStrValidMsg(HeaderNames.BandNumber, row[HeaderNames.BandNumber].ToString()));
        AppendIfNotEmpty(RequiredStrValidMsg(HeaderNames.CategoryCode, row[HeaderNames.CategoryCode].ToString()));
        AppendIfNotEmpty(RequiredStrValidMsg(HeaderNames.Manufacturer, row[HeaderNames.Manufacturer].ToString()));
        AppendIfNotEmpty(RequiredStrValidMsg(HeaderNames.PartSku, row[HeaderNames.PartSku].ToString()));

        AppendIfNotEmpty(ValidDecimalValMsg(HeaderNames.ListPrice, row[HeaderNames.ListPrice].ToString()));
        AppendIfNotEmpty(ValidDecimalValMsg(HeaderNames.MinDiscount, row[HeaderNames.MinDiscount].ToString()));
        AppendIfNotEmpty(ValidDecimalValMsg(HeaderNames.DiscountPrice, row[HeaderNames.DiscountPrice].ToString()));

        if (decimal.TryParse(row[HeaderNames.ListPrice].ToString(), out var listPrice) &&
            decimal.TryParse(row[HeaderNames.DiscountPrice].ToString(), out var discountPrice) &&
            decimal.TryParse(row[HeaderNames.MinDiscount].ToString(), out var minDiscount))
        {
            AppendIfNotEmpty(NegativeValMsg(HeaderNames.ListPrice, listPrice));
            AppendIfNotEmpty(NegativeValMsg(HeaderNames.DiscountPrice, discountPrice));
            AppendIfNotEmpty(NegativeValMsg(HeaderNames.MinDiscount, minDiscount));

            if (discountPrice > listPrice)
                errMessages.AppendLine($"Error in {rowNo} line: DiscountPrice should be smaller than ListPrice.");
            if (minDiscount < 0 || minDiscount > 100)
                errMessages.AppendLine($"Error in {rowNo} line: MinDiscount should be between 0 and 100.");
        }

        string RequiredStrValidMsg(string colName, string colVal)
            => !string.IsNullOrEmpty(colVal) ? string.Empty : $"Error in {rowNo} line {colName} cannot be null or empty.";
        
        string ValidDecimalValMsg(string colName, string colVal)
            => decimal.TryParse(colVal, out var price) ? string.Empty : $"Error in {rowNo} line, Invalid {colName}.";
         
        string NegativeValMsg(string colName, decimal colVal)
            => (colVal < 0) ? $"Error in {rowNo} line {colName} cannot be negative." : string.Empty;

        void AppendIfNotEmpty(string message)
        {
            if (!string.IsNullOrEmpty(message)) errMessages.AppendLine(message);
        }

        return errMessages.ToString();
    }
}
