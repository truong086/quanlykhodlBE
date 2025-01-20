namespace quanlykhodl.ViewModel
{
    public class PlanDTO
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public string? status { get; set; }
        public int? localtionNew { get; set; }
        public int? productlocation_map { get; set; }
        public int? Receiver { get; set; }
        public int? warehouse { get; set; }
        public int? area { get; set; }
        public int? floor { get; set; }
    }

    public class ConfirmationPlan
    {
        public int id { get; set; }
        public bool isConfirmation { get; set; }
    }

    public class SearchDateTime
    {
        public DateTime? date { get; set; }
    }
}
