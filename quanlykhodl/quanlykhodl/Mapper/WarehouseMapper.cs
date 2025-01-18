using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class WarehouseMapper : Profile
    {
        public WarehouseMapper()
        {
            CreateMap<WarehouseDTO, Warehouse>();
            CreateMap<Warehouse, WarehouseDTO>();
            CreateMap<Warehouse, WarehouseGetAll>();
        }
    }
}
