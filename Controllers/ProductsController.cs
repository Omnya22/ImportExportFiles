using ImportExportFiles.Interfaces;
using ImportExportFiles.Services;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace ImportExportFiles.Controllers;

public class ProductsController(
    IProductService productService,
    IImportService importService,
    IExportService exportService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? page, string search)
    {
        var products = await productService.SearchProductsAsync(search);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        return View(products.ToPagedList(pageNumber, pageSize));
    }
    [HttpGet]
    public IActionResult Import() => View();

    [HttpPost]
    public async Task<IActionResult> Import([FromForm] IFormFile file)
    {
        await importService.ImportDataFromExcelFileAsync(file);
        return RedirectToAction("Index");
    }
    [HttpGet]
    public async Task<IActionResult> Export(string search)
    {
        var productList = await productService.SearchProductsAsync(search);
        var fileBytes = exportService.ExportDataAsExcelFile("Products", productList);

        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Products.xlsx");
    }
}
