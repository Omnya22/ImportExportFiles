﻿using ImportExportFiles.Models;

namespace ImportExportFiles.Interfaces;
public interface IProductService
{
    Task<IEnumerable<ProductViewModel>> SearchProductsAsync(string search);
    Task AddOrUpdateProductAsync(ProductViewModel product);
    Task AddOrUpdateProductsAsync(IEnumerable<ProductViewModel> products);
    Task InsertBatchAsync(IEnumerable<ProductViewModel> products);
}
