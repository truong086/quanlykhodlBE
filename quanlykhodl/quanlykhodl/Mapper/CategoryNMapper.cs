using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class CategoryNMapper : Profile
    {
        public CategoryNMapper()
        {
            CreateMap<CategoryDTO, categories>();
            CreateMap<categories, CategoryDTO>();
            CreateMap<categories, CategoryGetAll>();
        }
    }
}
          