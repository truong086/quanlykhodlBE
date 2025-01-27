using AutoMapper;
using quanlykhodl.Common;
using quanlykhodl.Models;

namespace quanlykhodl.Mapper
{
    public class OnlineUsersMapper : Profile
    {
        public OnlineUsersMapper()
        {
            CreateMap<OnlineUsers, GetAllAccountOnline>();
            CreateMap<GetAllAccountOnline, OnlineUsers>();
        }
    }
}
