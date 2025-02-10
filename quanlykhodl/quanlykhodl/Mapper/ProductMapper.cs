using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class ProductMapper : Profile
    {
        public ProductMapper()
        {
            CreateMap<product, ProductGetAll>();
            CreateMap<ProductDTO, product>();
            CreateMap<product, ProductDTO>();
            CreateMap<product, ProductOneLocation>();
            CreateMap<product, productNew>();
            CreateMap<productNew, product>();
            CreateMap<product, productImportformAndDeliveerrynote>();
            CreateMap<product, PrepareToExportGetAll>();
            CreateMap<product, productWarehouse>();
        }
    }
}
