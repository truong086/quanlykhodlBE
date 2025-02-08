using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class categories: BaseEntity
    {
        public string? name {  get; set; }
        public string? image { get; set; }
        public string? public_id { get; set; }
        public int? account_id { get; set; }
        public accounts? account { get; set; }

        public ICollection<product>? products { get; set; }
    }
}
