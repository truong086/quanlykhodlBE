using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class producthisstorylocation : BaseEntity
    {
        public int ? plan_id {  get; set; }
        public int ? product_old {  get; set; }
        public int? product_id { get; set; }
        public int? locationold { get; set; }
        public int? locationnew { get; set; }
        public int? shelfold { get; set; }
        public int? shelfnew { get; set; }
        public product? products { get; set; }
        public Plan? plans { get; set; }
    }
}
