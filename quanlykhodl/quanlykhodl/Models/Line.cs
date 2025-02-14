using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class Line : BaseEntity
    {
        public string? name { get; set; }
        public int quantityshelf {  get; set; }
        public int? id_area { get; set; }
        public areas? areasid { get; set; }
        public ICollection<Shelf>? shelfs { get; set; }
    }
}
