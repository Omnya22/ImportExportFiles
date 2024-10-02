using ImportExportFiles.Data;
using ImportExportFiles.Data.Entities;
using ImportExportFiles.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ImportExportFiles.Repositories;

public class ProductRepository(ApplicationDbContext context) : IProductRepository
{
    public async Task AddAsync(Product product)
    {
        await context.Products.AddAsync(product);
    }
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await context.Products.ToListAsync();
    }
    public async Task<Product> GetBySkuAsync(string sku)
    => await context.Products.FirstOrDefaultAsync(p => p.PartSku == sku);
    public async Task<IEnumerable<Product>> SearchAsync(string searchTerm)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return await context.Products.ToListAsync();

        _ = decimal.TryParse(searchTerm, out decimal decimalSearch);
        return await context.Products
            .Where(p => p.BandNumber.Contains(searchTerm) ||
                    p.CategoryCode.Contains(searchTerm) ||
                    p.Manufacturer.Contains(searchTerm) ||
                    p.PartSku.Contains(searchTerm) ||
                    p.ItemDescription.Contains(searchTerm) ||
                    p.ListPrice == decimalSearch ||
                    p.MinDiscount == decimalSearch ||
                    p.DiscountPrice == decimalSearch).ToListAsync();

    }
    public void Update(Product product)
    {
        context.Products.Update(product);
    }
}
