using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class LocationException : BaseEntity
    {
        public int id_shelf { get; set; }
        public Shelf? shelf { get; set; }
        public int? location { get; set; }
        public int? max { get; set; }

    }
}
