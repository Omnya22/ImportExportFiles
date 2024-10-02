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
    public async Task AddRangesAsync(IEnumerable<Product> products)
    {
        List<Product> productsToUpdate = [];
        List<Product> productsToInsert = [];

        var partSkus = products.Select(p => p.PartSku).ToList();
        var existingProducts = await context.Products.Where(p => partSkus.Contains(p.PartSku)).ToListAsync();

        foreach (var product in products)
        {
            var existingProduct = existingProducts.FirstOrDefault(p => p.PartSku == product.PartSku);
            if (existingProduct != null)
            {
                existingProduct.BandNumber = product.BandNumber;
                existingProduct.CategoryCode = product.CategoryCode;
                existingProduct.Manufacturer = product.Manufacturer;
                existingProduct.ItemDescription = product.ItemDescription;
                existingProduct.ListPrice = product.ListPrice;
                existingProduct.MinDiscount = product.MinDiscount;
                existingProduct.DiscountPrice = product.DiscountPrice;

                productsToUpdate.Add(existingProduct);
            }
            else
                productsToInsert.Add(product);
        }

        if (productsToInsert.Any())
            await context.Products.AddRangeAsync(productsToInsert);

        if (productsToUpdate.Any())
            context.Products.UpdateRange(productsToUpdate);

        await context.SaveChangesAsync();
    }
}
