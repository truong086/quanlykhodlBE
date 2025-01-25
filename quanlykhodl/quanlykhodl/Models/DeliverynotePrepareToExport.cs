using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class DeliverynotePrepareToExport : BaseEntity
    {
        public int? id_PrepareToExport {  get; set; }
        public PrepareToExport? PreparetoExports {  get; set; }
        public string? code { get; set; }
        public int? id_delivenote {  get; set; }
        public Deliverynote? deliverynotes {  get; set; }

    }
}
