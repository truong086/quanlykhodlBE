using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class areas : BaseEntity
    {
        public string? name { get; set; }
        public string? code { get; set; }
        public string? status { get; set; }
        public string? image { get; set; }
        public string? publicId { get; set; }
        public int storage { get; set; }
        public int? floor { get; set; }
        public int? account_id { get; set; }
        public Floor? floor_id { get; set; }
        public accounts? account { get; set; }

        public ICollection<Shelf>? shelfsl { get; set; }
        public ICollection<Plan>? plans { get; set; }

    }
}
