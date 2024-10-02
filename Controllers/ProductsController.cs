using ImportExportFiles.Interfaces;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace ImportExportFiles.Controllers;

public class ProductsController(IProductService productService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? page, string search)
    {
        var products = await productService.SearchProductsAsync(search);
        int pageSize = 10;
        int pageNumber = (page ?? 1);
        return View(products.ToPagedList(pageNumber, pageSize));
    }
    public IActionResult Import()
    {
        return View();
    }
}
