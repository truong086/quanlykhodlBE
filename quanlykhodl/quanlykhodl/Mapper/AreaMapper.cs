using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class AreaMapper : Profile
    {
        public AreaMapper()
        {
            CreateMap<areas, AreaDTO>();
            CreateMap<AreaDTO, areas>();
            CreateMap<areas, AreaGetAll>();
        }
    }
}
