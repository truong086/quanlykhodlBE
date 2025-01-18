using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Models;
using Quartz;

namespace quanlykhodl.QuartzService
{
    public class TuDongMoiTuan : IJob
    {
        private readonly DBContext _dbContext;
        private readonly Cloud _cloud;
        public TuDongMoiTuan(DBContext context, IOptions<Cloud> cloud)
        {
            _dbContext = context;
            _cloud = cloud.Value;
        }
        public Task Execute(IJobExecutionContext context)
        { 
            var checkAccount = _dbContext.accounts.Where(x => !x.Deleted && !x.Action).ToList();
            LoadAccount(checkAccount);

            return Task.CompletedTask;
        }

        private void LoadAccount(List<Account> accounts)
        {
            if(accounts.Any())
            {
                foreach(var item in accounts)
                {
                    var checkToken = _dbContext.tokens.Include(a => a.account).Where(x => x.account_id == item.id)
                        .OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                    if(checkToken != null)
                    {
                        var checkDate = checkDateChenhLech(checkToken.CreatedAt);
                        if(checkDate >= 1)
                        {
                            var deleteToken = _dbContext.tokens.Include(a => a.account)
                                .Where(x => x.account_id == item.id).ToList();

                            uploadCloud.DeleteAllImageAndFolder(item.email, _cloud);
                            _dbContext.tokens.RemoveRange(deleteToken);
                            _dbContext.accounts.Remove(item);

                            _dbContext.SaveChanges();
                        }
                    }
                }
            }
        }

        private int checkDateChenhLech(DateTimeOffset data)
        {
            var dateNow = DateTimeOffset.UtcNow;

            TimeSpan tinhChenhLech = dateNow.Subtract(data);

            int chuyenDoi = Math.Abs(tinhChenhLech.Minutes);
            /*
                "Days": Ngày
                "Hours": Giờ
                "Minutes": Phút
                "Seconds": Giây
             */

            return chuyenDoi;


        }
    }
}
