using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class Linemapper : Profile
    {
        public Linemapper()
        {
            CreateMap<LineDTO, Line>();
            CreateMap<Line, LineDTO>();
            CreateMap<Line, LinegetAll>();
        }
    }
}
