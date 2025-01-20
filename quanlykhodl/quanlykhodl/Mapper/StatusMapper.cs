using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class StatusMapper : Profile
    {
        public StatusMapper()
        {
            CreateMap<StatusItemDTO, StatusItem>();
            CreateMap<StatusItem, StatusItemDTO>();
        }
    }
}
