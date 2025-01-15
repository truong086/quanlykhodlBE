using quanlykhodl.Models;

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
            Console.WriteLine("Test");
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine("Test");
        }
    }
}
