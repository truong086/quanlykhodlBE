using System.ComponentModel.DataAnnotations;

namespace quanlykhodl.Common
{
    public class BaseEntity
    {
        protected BaseEntity() { }
        [Key]
        public int id { get; set; }
        public bool deleted { get; set; }
        public string? cretoredit { get; set; }
        public DateTimeOffset createdat { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset updatedat { get; set; }
    }
}
