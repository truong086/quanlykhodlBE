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
                var checkAccount = _context.accounts.Where(x => x.id == int.Parse(user) && !x.deleted).FirstOrDefault();
                var checkAccountUser1 = _context.accounts.Where(x => x.id == userId1 && !x.deleted).FirstOrDefault();
                if (checkAccount == null || checkAccountUser1 == null)
                    return await Task.FromResult(PayLoad<object>.CreatedFail(Status.DATANULL));

                var data = _context.messages.Where(x => (x.senderid == checkAccountUser1.id && x.receiverid == checkAccount.id) ||
                (x.senderid == checkAccount.id && x.receiverid == checkAccountUser1.id)).OrderBy(x => x.createdat).ToList();

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
                        idUser1 = findOneAccount(item.receiverid.Value).id,
                        image_user1 = findOneAccount(item.receiverid.Value).image,
                        name_user1 = findOneAccount(item.receiverid.Value).username,
                        idUser2 = findOneAccount(item.senderid.Value).id,
                        name_user2 = findOneAccount(item.senderid.Value).username,
                        image_user2 = findOneAccount(item.senderid.Value).image,
                        message = item.content,
                        imagedata = item.image == null || item.image == "" ? null : item.image,
                        CreateAt = item.createdat
                    };

                    list.Add(dataItem);
                }
            }

            return list;
        }

        private accounts findOneAccount(int id)
        {
            var checkAccount = _context.accounts.Where(x => x.id == id && !x.deleted).FirstOrDefault();
            return checkAccount;
        }
    }
}
