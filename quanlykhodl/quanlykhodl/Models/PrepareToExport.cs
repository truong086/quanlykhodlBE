using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class PrepareToExport : BaseEntity
    {
        public int? id_product { get; set; }
        public product? product { get; set; }
        public int quantity {  get; set; }
        public string? code {  get; set; }
        public int? account_id {  get; set; }
        public bool ischeck {  get; set; }
        public accounts? account {  get; set; }
        public virtual ICollection<DeliverynotePrepareToExport>? DeliverynotePrepareToExports { get; set; }
    }
}