using AutoMapper;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Mapper
{
    public class AccountMapper : Profile
    {
        public AccountMapper()
        {
            CreateMap<AccountDTO, accounts>();
            CreateMap<RoleDTO, role>();
            CreateMap<accounts, AccountgetAll>();
            CreateMap<AccountgetAll, accounts>();
        }
    }
}
