using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class Codelocation : BaseEntity
    {
        public int? id_helf {  get; set; }
        public Shelf? shelf {  get; set; }
        public int location {  get; set; }
        public string? code {  get; set; }
    }
}
