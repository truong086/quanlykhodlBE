using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class category: BaseEntity
    {
        public string? name {  get; set; }
        public string? image { get; set; }
        public string? public_id { get; set; }
        public int? account_Id { get; set; }
        public Account? account { get; set; }

        public ICollection<product>? products { get; set; }
    }
}
