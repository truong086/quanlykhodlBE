using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class LocationException : BaseEntity
    {
        public int id_area { get; set; }
        public Area? area { get; set; }
        public int? location { get; set; }
        public int? max { get; set; }

    }
}
