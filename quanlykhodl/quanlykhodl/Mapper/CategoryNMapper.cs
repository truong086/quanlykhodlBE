using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class CategoryNMapper : Profile
    {
        public CategoryNMapper()
        {
            CreateMap<CategoryDTO, category>();
            CreateMap<category, CategoryDTO>();
            CreateMap<category, CategoryGetAll>();
        }
    }
}
          