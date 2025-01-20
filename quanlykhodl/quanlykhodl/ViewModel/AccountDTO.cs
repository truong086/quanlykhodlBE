using quanlykhodl.Models;

namespace quanlykhodl.ViewModel
{
    public class AccountDTO
    {
        public string? username { get; set; }
        public string? password { get; set; }
        public IFormFile? image { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? address { get; set; }
    }

    public class AccountUpdate
    {
        public string? username { get; set; }
        public IFormFile? image { get; set; }
        public string? email { get; set; }
        public string? phone { get; set; }
        public string? address { get; set; }
        public int role_id { get; set; }
    }


    public class AccountUpdateRole
    {
        public int role_id { get; set; }
        public int account_id { get; set; }
    }
}
