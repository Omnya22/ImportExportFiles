using ImportExportFiles.Data;
using ImportExportFiles.Interfaces;
using ImportExportFiles.Repositories;

namespace ImportExportFiles.UOW;

public class UnitOfWork(ApplicationDbContext context, IProductRepository productRepository) : IUnitOfWork
{
    public IProductRepository Products => productRepository ??= new ProductRepository(context);
    public async Task<int> CompleteAsync() => await context.SaveChangesAsync();
    public void Dispose() => context.Dispose();
}

