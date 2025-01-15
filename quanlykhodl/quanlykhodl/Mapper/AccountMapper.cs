using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<AccountDTO, Account>();
        }
    }
}
