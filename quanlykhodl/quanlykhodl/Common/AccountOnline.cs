using AutoMapper;
using Microsoft.EntityFrameworkCore;
using quanlykhodl.Models;

namespace quanlykhodl.Common
{
    public static class AccountOnline
    {
        public static IEnumerable<object> GetAll(List<OnlineUsers> data, IMapper mapper, DBContext context)
        {
            var list = new List<GetAllAccountOnline>();
            if (data.Any())
            {
                foreach(var item in data)
                {
                    list.Add(findOne(item, mapper, context));
                }
            }

            var dataMaper = list.Select(x => new
            {
                id = x.id,
                id_account = x.id_account,
                ConnectId = x.ConnectionId,
                isOnline = x.IsOnline,
                account_name = x.username,
                account_image = x.image
            });
            return dataMaper;
        }

        private static GetAllAccountOnline findOne(OnlineUsers dataItem, IMapper mapper, DBContext context)
        {
            var checkAccount = context.accounts.Where(x => x.id == dataItem.account_id && !x.deleted).FirstOrDefault();
            var dataMap = mapper.Map<GetAllAccountOnline>(dataItem);
            if(checkAccount != null)
            {
                dataMap.id = dataItem.id;
                dataMap.id_account = checkAccount.id;
                dataMap.username = checkAccount.username;
                dataMap.image = checkAccount.image;
            }

            return dataMap;
        }
    }

    public class GetAllAccountOnline
    {
        public int id { get; set; }
        public int id_account { get; set; }
        public string? ConnectionId { get; set; }
        public bool IsOnline { get; set; }
        public string? username { get; set; }
        public string? image { get; set; }
    }
}
