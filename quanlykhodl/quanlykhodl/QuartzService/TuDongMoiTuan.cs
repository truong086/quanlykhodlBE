using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using quanlykhodl.Clouds;
using quanlykhodl.Models;
using quanlykhodl.ViewModel;
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
            var checkAccountActionUpdatePassword = _dbContext.accounts.Where(x => !x.Deleted && x.Action).ToList();
            LoadAccount(checkAccount);

            return Task.CompletedTask;
        }

        private void LoadAccount(List<Account> accounts)
        {
            if(accounts.Any())
            {
                foreach(var item in accounts)
                {
                    var checkTokenCreate = _dbContext.tokens.Include(a => a.account).Where(x => x.account_id == item.id && x.Status == Status.CREATEPASSWORD)
                        .OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                    if (checkTokenCreate != null)
                    {
                        var checkDate = checkDateChenhLech(checkTokenCreate.CreatedAt);
                        if(checkDate >= 1)
                        {
                            var deleteToken = _dbContext.tokens.Include(a => a.account)
                                .Where(x => x.account_id == item.id && x.Status == Status.CREATEPASSWORD).ToList();

                            uploadCloud.DeleteAllImageAndFolder(item.email, _cloud);
                            _dbContext.tokens.RemoveRange(deleteToken);
                            _dbContext.accounts.Remove(item);

                            _dbContext.SaveChanges();
                        }
                    }
                }
            }
        }
        private void LoadAccountActionUpdatePassword(List<Account> data)
        {
            if (data.Any())
            {
                foreach(var item in data)
                {
                    var checkTokenUpdate = _dbContext.tokens.Include(a => a.account).Where(x => x.account_id == item.id && x.Status == Status.UPDATEPASSWORD)
                        .OrderByDescending(x => x.CreatedAt).FirstOrDefault();

                    if(checkTokenUpdate != null)
                    {
                        var checkDate = checkDateChenhLech(checkTokenUpdate.CreatedAt);
                        if (checkDate >= 30)
                        {
                            var deleteToken = _dbContext.tokens.Include(a => a.account)
                                .Where(x => x.account_id == item.id && x.Status == Status.UPDATEPASSWORD).ToList();

                            _dbContext.tokens.RemoveRange(deleteToken);

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
