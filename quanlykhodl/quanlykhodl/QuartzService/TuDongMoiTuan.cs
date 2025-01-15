using quanlykhodl.Models;
using Quartz;

namespace quanlykhodl.QuartzService
{
    public class TuDongMoiTuan : IJob
    {
        private readonly DBContext _dbContext;
        public TuDongMoiTuan(DBContext context)
        {
            _dbContext = context;
        }
        public Task Execute(IJobExecutionContext context)
        { 
            return Task.CompletedTask;
        }
    }
}
