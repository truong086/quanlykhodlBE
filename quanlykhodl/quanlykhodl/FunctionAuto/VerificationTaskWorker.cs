using quanlykhodl.Models;
using quanlykhodl.Service;
using quanlykhodl.ViewModel;

namespace quanlykhodl.FunctionAuto
{
    public class VerificationTaskWorker : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        public VerificationTaskWorker(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        // Đây là hàm gọi tự động
        public async Task ProcessVerificationAsync(Token token, string? status)
        {
            try
            {
                // Đợi 1 phút
                await Task.Delay(TimeSpan.FromMinutes(1));

                // Đợi 10 giây
                //await Task.Delay(TimeSpan.FromSeconds(10));
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var accountServiceDelete = scope.ServiceProvider.GetRequiredService<IAccountService>();
                    if (status == Status.CREATEPASSWORD)
                    {
                        // Sau 1 phút sẽ gọi đến hàm "DeleteToken"
                        var result = await accountServiceDelete.DeleteToken(token, Status.CREATEPASSWORD);
                        //await accountServiceDelete.DeleteAccountNoAction();
                    }
                    else if (status == Status.UPDATEPASSWORD)
                    {
                        // Sau 1 phút sẽ gọi đến hàm "DeleteToken"
                        var result = await accountServiceDelete.DeleteToken(token, Status.UPDATEPASSWORD);
                        //await accountServiceDelete.DeleteAccountNoAction();
                    }

                    //Console.WriteLine(result);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var accountService = scope.ServiceProvider.GetRequiredService<IAccountService>();

                        // Gọi hàm DeleteAccountNoAction
                        await accountService.DeleteAccountNoAction();
                    }

                    // Tùy chỉnh thời gian lặp lại
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Lỗi: {ex.Message}");
                }
            }
        }
    }
}
