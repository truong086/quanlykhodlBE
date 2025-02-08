using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class productlocation : BaseEntity
    {
        public int location {  get; set; }
        public int id_product {  get; set; }
        public int id_shelf {  get; set; }
        public bool isaction {  get; set; }
        public product? products { get; set; }
        public Shelf? shelfs { get; set; }
        public int quantity { get; set; }
        public string? codelocation { get; set; }
        public virtual ICollection<Plan>? plans { get; set; }
        public virtual ICollection<productDeliverynote>? ProductDeliverynotes { get; set; }
    }
}
