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
            var checkAccount = _dbContext.accounts.Where(x => !x.deleted && !x.action).ToList();
            if(checkAccount != null && checkAccount.Count > 0)
            {
                var checkAccountActionUpdatePassword = _dbContext.accounts.Where(x => !x.deleted && x.action).ToList();
                LoadAccount(checkAccount);
            }
            

            return Task.CompletedTask;
        }

        private void LoadAccount(List<accounts> accounts)
        {
            if(accounts.Any())
            {
                var listAccount = new List<accounts>();
                foreach(var item in accounts)
                {
                    var checkTokenCreate = _dbContext.tokens.Include(a => a.account).Where(x => x.account_id == item.id && x.status == Status.CREATEPASSWORD)
                        .OrderByDescending(x => x.createdat).FirstOrDefault();

                    var checkDateAccount = checkDateChenhLech(item.createdat);
                    if (checkTokenCreate != null)
                    {
                        var checkDate = checkDateChenhLech(checkTokenCreate.createdat);
                        
                        if (checkDate >= 1)
                        {
                            var deleteTokens = _dbContext.tokens.Include(a => a.account)
                                .Where(x => x.account_id == item.id && x.status == Status.CREATEPASSWORD).ToList();

                            uploadCloud.DeleteAllImageAndFolder(item.email, _cloud);
                            if (deleteTokens.Count > 0)
                                deleteToken(deleteTokens);

                            if (checkDateAccount >= 5)
                            {
                                if (item != null)
                                    deleteAccount(item);
                            }
                        }
                    }
                    else
                    {
                        if (checkDateAccount >= 5)
                        {
                            if (item != null)
                                deleteAccount(item);
                        }
                    }
                    
                }
            }
        }

        private void deleteAccount(accounts account)
        {
            if(account != null)
            {
                _dbContext.accounts.Remove(account);
                _dbContext.SaveChanges();
            }
            
        }

        private void deleteToken(List<Token> tokens)
        {
            if(tokens.Count > 0 && tokens != null && tokens.Any())
            {
                _dbContext.tokens.RemoveRange(tokens);
                _dbContext.SaveChanges();
            }
        }
        private void LoadAccountActionUpdatePassword(List<accounts> data)
        {
            if (data.Any())
            {
                foreach(var item in data)
                {
                    var checkTokenUpdate = _dbContext.tokens.Include(a => a.account).Where(x => x.account_id == item.id && x.status == Status.UPDATEPASSWORD)
                        .OrderByDescending(x => x.createdat).FirstOrDefault();

                    if(checkTokenUpdate != null)
                    {
                        var checkDate = checkDateChenhLech(checkTokenUpdate.createdat);
                        if (checkDate >= 30)
                        {
                            var deleteToken = _dbContext.tokens.Include(a => a.account)
                                .Where(x => x.account_id == item.id && x.status == Status.UPDATEPASSWORD).ToList();

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
