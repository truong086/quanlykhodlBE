using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class ImportformMapper : Profile
    {
        public ImportformMapper()
        {
            CreateMap<Importform, ImportformGetAll>();
            CreateMap<ImportformDTO, Importform>();
            CreateMap<Importform, ImportformDTO>();
        }
    }
}
