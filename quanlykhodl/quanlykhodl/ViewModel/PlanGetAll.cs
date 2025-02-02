namespace quanlykhodl.ViewModel
{
    public class PlanGetAll
    {
        public int Id { get; set; }
        public string? title { get; set; }
        public string? description { get; set; }
        public string? status { get; set; }
        public int? localtionNew { get; set; }
        public bool isConfirmation { get; set; }
        public bool isConsent { get; set; }
        public string? productName { get; set; }
        public string? productImage { get; set; }
        public string? Receiver_name { get; set; }
        public string? Receiver_image { get; set; }
        public int? localtionOld { get; set; }
        public string? localtionOldCode { get; set; }
        public string? localtionNewCode { get; set; }
        public string? warehouseOld { get; set; }
        public string? areaOld { get; set; }
        public string? floorOld { get; set; }
        public string? warehouse { get; set; }
        public string? area { get; set; }
        public string? floor { get; set; }
        public string? Account_creatPlan { get; set; }
        public string? CodeWarehourseNew { get; set; }
        public string? CodeFloorNew { get; set; }
        public string? CodeAreaeNew { get; set; }
        public string? CodeWarehourseOld { get; set; }
        public string? CodeFloorOld { get; set; }
        public string? CodeAreaeOld { get; set; }
        public string? ImageFloorNew { get; set; }
        public string? ImageAreaeNew { get; set; }
        public string? ImageWarehourseOld { get; set; }
        public string? ImageWarehourseNew { get; set; }
        public string? ImageFloorOld { get; set; }
        public string? ImageAreaeOld { get; set; }
        public DateTimeOffset? UpdatedAt { get; set; }
    }
}
