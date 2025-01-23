using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class PrepareToExport : BaseEntity
    {
        public int? id_product { get; set; }
        public product? product { get; set; }
        public int quantity {  get; set; }
        public int? account_id {  get; set; }
        public Account? account {  get; set; }
        public virtual ICollection<DeliverynotePrepareToExport>? DeliverynotePrepareToExports { get; set; }
    }
}
