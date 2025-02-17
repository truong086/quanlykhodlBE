namespace quanlykhodl.ViewModel
{
    public class DataExcelLocationProduct
    {
        public string? warehouseID { get; set; }
        public int ptNo { get; set; }
        public DateTimeOffset? ptLimitDate { get; set; }
        public string? wsNo { get; set; }
        public bool? wsMode { get; set; }
        public int wsNum { get; set; }
        public int osID { get; set; }
        public string? wsArea { get; set; }
        public string? wsRow { get; set; }
        public string? wsFrame { get; set; }
        public DateTimeOffset? CreateDate { get; set; }
        public DateTimeOffset? LastModifyDate { get; set; }

    }
}
