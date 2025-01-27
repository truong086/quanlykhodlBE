using quanlykhodl.Common;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;

namespace quanlykhodl.Service
{
    public class MessageService : IMessageService
    {
        private readonly DBContext _context;
        private readonly IUserService _userService;
        public MessageService(DBContext context, IUserService userService)
        {
            _userService = userService;
            _context = context;
        }
        public async Task<PayLoad<object>> FindAll(int userId1)
        {
            try
            {
                var user = _userService.name();
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.Deleted).FirstOrDefault();
                var checkAccountUser1 = _context.accounts.Where(x => x.id == userId1 && !x.Deleted).FirstOrDefault();
                if (checkAccount == null || checkAccountUser1 == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var data = _context.Messages.Where(x => (x.SenderId == checkAccountUser1.id && x.ReceiverId == checkAccount.id) ||
                (x.SenderId == checkAccount.id && x.ReceiverId == checkAccountUser1.id)).OrderBy(x => x.CreatedAt).ToList();

                var dataMap = new MessageGetAll
                {
                    id = checkAccountUser1.id,
                    username = checkAccountUser1.username,
                    image = checkAccountUser1.image,
                    dataItem = loadData(data)
                };

                return await Task.FromResult(PayLoad<object>.Successfully(dataMap));
            }
            catch(Exception ex)
            {
                return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));
            }
        }

        private List<MessageItem> loadData(List<Message> data)
        {
            var list = new List<MessageItem>();

            if (data.Any())
            {
                foreach(var item in data)
                {
                    var dataItem = new MessageItem
                    {
                        idUser1 = findOneAccount(item.ReceiverId.Value).id,
                        image_user1 = findOneAccount(item.ReceiverId.Value).image,
                        name_user1 = findOneAccount(item.ReceiverId.Value).username,
                        idUser2 = findOneAccount(item.SenderId.Value).id,
                        name_user2 = findOneAccount(item.SenderId.Value).username,
                        image_user2 = findOneAccount(item.SenderId.Value).image,
                        message = item.Content,
                        imagedata = item.image == null || item.image == "" ? null : item.image,
                        CreateAt = item.CreatedAt
                    };

                    list.Add(dataItem);
                }
            }

            return list;
        }

        private Account findOneAccount(int id)
        {
            var checkAccount = _context.accounts.Where(x => x.id == id && !x.Deleted).FirstOrDefault();
            return checkAccount;
        }
    }
}
