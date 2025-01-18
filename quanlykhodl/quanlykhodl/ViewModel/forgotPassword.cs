namespace quanlykhodl.ViewModel
{
    public class forgotPassword
    {
        public string? email { get; set; }
        public string? passwordOld { get; set; }
        public string? passwordNew { get; set; }
        public string? code { get; set; }
    }

    public class ActionAccount
    {
        public string? email { get; set; }
        public string? code { get; set; }
    }

    public class updatatePasswordAccount
    {
        public string? email { get; set; }
        public string? passwordNew { get; set; }
        public string? code { get; set; }
    }
}
