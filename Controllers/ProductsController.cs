using Microsoft.AspNetCore.Mvc;

namespace ImportExportFiles.Controllers;

public class ProductsController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
    public IActionResult Import()
    {
        return View();
    }
}
