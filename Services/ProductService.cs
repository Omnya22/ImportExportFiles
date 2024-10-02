using AutoMapper;
using ImportExportFiles.Interfaces;
using ImportExportFiles.Models;

namespace ImportExportFiles.Services;

public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string search)
    {
        var products = await unitOfWork.Products.SearchAsync(search);
        return mapper.Map<List<ProductViewModel>>(products);
    }
}
