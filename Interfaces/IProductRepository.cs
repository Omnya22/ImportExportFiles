using ImportExportFiles.Data.Entities;

namespace ImportExportFiles.Interfaces;

public interface IProductRepository : IRepository<Product>
{
    Task<Product> GetBySkuAsync(string sku);
    Task<IEnumerable<Product>> SearchAsync(string searchTerm);
}
