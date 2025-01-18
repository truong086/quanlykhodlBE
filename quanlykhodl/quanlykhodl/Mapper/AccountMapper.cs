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
            CreateMap<RoleDTO, role>();
            CreateMap<Account, AccountgetAll>();
            CreateMap<AccountgetAll, Account>();
        }
    }
}
