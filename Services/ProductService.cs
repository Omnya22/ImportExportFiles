using AutoMapper;
using ImportExportFiles.Data.Entities;
using ImportExportFiles.Interfaces;
using ImportExportFiles.Models;

namespace ImportExportFiles.Services;

public class ProductService(IUnitOfWork unitOfWork, IMapper mapper) : IProductService
{
    public async Task AddOrUpdateProductAsync(ProductViewModel product)
    {
        var existingProduct = await unitOfWork.Products.GetBySkuAsync(product.PartSku);

        if (existingProduct == null)
        {
            var newProduct = new Product
            {
                BandNumber = product.BandNumber,
                CategoryCode = product.CategoryCode,
                Manufacturer = product.Manufacturer,
                PartSku = product.PartSku,
                ItemDescription = product.ItemDescription,
                ListPrice = product.ListPrice,
                MinDiscount = product.MinDiscount,
                DiscountPrice = product.DiscountPrice
            };
            await unitOfWork.Products.AddAsync(newProduct);
        }
        else
        {
            existingProduct.BandNumber = product.BandNumber;
            existingProduct.CategoryCode = product.CategoryCode;
            existingProduct.Manufacturer = product.Manufacturer;
            existingProduct.ItemDescription = product.ItemDescription;
            existingProduct.ListPrice = product.ListPrice;
            existingProduct.MinDiscount = product.MinDiscount;
            existingProduct.DiscountPrice = product.DiscountPrice;

            unitOfWork.Products.Update(existingProduct);            
        }
        await unitOfWork.CompleteAsync();
    }
    public async Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string search)
    {
        var products = await unitOfWork.Products.SearchAsync(search);
        return mapper.Map<List<ProductViewModel>>(products);
    }
}
