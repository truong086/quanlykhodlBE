using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class PlanMapper : Profile
    {
        public PlanMapper()
        {
            CreateMap<PlanDTO, Plan>();
            CreateMap<Plan, PlanDTO>();
            CreateMap<Plan, PlanGetAll>();
            CreateMap<Plan, PlanAllWarehoursDTO>();
        }
    }
}
