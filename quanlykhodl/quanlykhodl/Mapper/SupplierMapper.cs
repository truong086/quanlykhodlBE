using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class SupplierMapper : Profile
    {
        public SupplierMapper()
        {
            CreateMap<SupplierDTO, Supplier>();
            CreateMap<Supplier, SupplierDTO>();
            CreateMap<Supplier, SupplierGetAll>();
        }
    }
}
