using AutoMapper;
using ImportExportFiles.Data.Entities;
using ImportExportFiles.Models;

namespace ImportExportFiles.Services.Mapper;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<Product, ProductViewModel>();
        CreateMap<ProductViewModel, Product>();
    }
}
