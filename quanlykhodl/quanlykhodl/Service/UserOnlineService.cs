using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class UserOnlineService : IUserOnlineService
    {
        private readonly DBContext _context;
        public UserOnlineService(DBContext context)
        {
            _context = context;
        }
        public async Task<PayLoad<object>> FindAll()
        {
            try
            {
                var data = _context.onlineusersuser.Where(x => x.isonline).ToList();

                return await Task.FromResult(PayLoad<object>.Successfully(loadData(data)));
            }catch (Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(ex.Message));
            }
        }

        private List<UserOnlineGetAll> loadData(List<OnlineUsers> data)
        {
            var list = new List<UserOnlineGetAll>();

            foreach (var item in data)
            {
                var checkAccount = _context.accounts.Where(x => x.id == item.account_id && !x.deleted).FirstOrDefault();
                if(checkAccount != null)
                {
                    var dataItem = new UserOnlineGetAll
                    {
                        Account_image = checkAccount.image,
                        Account_name = checkAccount.username,
                        ConnectId = item.connectionid,
                        Id = item.account_id.Value,
                    };

                    list.Add(dataItem);
                }
            }

            return list;
        }
    }
}
