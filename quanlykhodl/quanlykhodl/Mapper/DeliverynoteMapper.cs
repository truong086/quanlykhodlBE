using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class DeliverynoteMapper : Profile
    {
        public DeliverynoteMapper()
        {
            CreateMap<Deliverynote, DeliverynoteGetAll>();
            CreateMap<DeliverynoteDTO, Deliverynote>();
            CreateMap<Deliverynote, DeliverynoteDTO>();

        }
    }
}
