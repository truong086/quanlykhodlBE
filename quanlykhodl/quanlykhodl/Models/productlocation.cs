using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class productlocation : BaseEntity
    {
        public int location {  get; set; }
        public int id_product {  get; set; }
        public int id_area {  get; set; }
        public product? products { get; set; }
        public Area? areas { get; set; }
        public int quantity { get; set; }
        public virtual ICollection<Plan>? plans { get; set; }
    }
}
