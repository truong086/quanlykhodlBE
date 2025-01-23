using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class Codelocation : BaseEntity
    {
        public int? id_area {  get; set; }
        public Area? area {  get; set; }
        public int location {  get; set; }
        public string? code {  get; set; }
    }
}
