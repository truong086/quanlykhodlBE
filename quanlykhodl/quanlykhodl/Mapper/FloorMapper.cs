using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class FloorMapper : Profile
    {
        public FloorMapper()
        {
            CreateMap<FloorDTO, Floor>();
            CreateMap<Floor, FloorDTO>();
            CreateMap<Floor, FloorGetAll>();
        }
    }
}
