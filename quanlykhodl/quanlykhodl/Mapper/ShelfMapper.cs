using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class ShelfMapper : Profile
    {
        public ShelfMapper()
        {
            CreateMap<ShelfDTO, Shelf>();
            CreateMap<Shelf, ShelfDTO>();
            CreateMap<Shelf, ShelfGetAll>();
        }
    }
}
