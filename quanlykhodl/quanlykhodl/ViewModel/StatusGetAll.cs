namespace quanlykhodl.ViewModel
{
    public class StatusGetAll
    {
        public int id_status {  get; set; }
        public int id_plan {  get; set; }
        public int id_product {  get; set; }
        public string? product_name {  get; set; }
        public string? product_iamge {  get; set; }
        public string? plan_tile {  get; set; }
        public int locationOld {  get; set; }
        public int locationNew {  get; set; }
        public string? shelfOld { get; set; }
        public string? shelfNew { get; set; }
        public string? areaOld {  get; set; }
        public string? areaNew {  get; set; }
        public string? FloorOld {  get; set; }
        public string? FloorNew {  get; set; }
        public string? WarehourseOld {  get; set; }
        public string? WarehourseNew {  get; set; }
        public string? StatusPlan {  get; set; }
        public string? Account_name {  get; set; }
        public string? Account_image {  get; set; }
        public string? CodeLocationOld {  get; set; }
        public string? CodeLocationNew {  get; set; }
        public List<StatusItemPlan>? statusItemPlans { get; set; }
    }

    public class StatusItemPlan
    {
        public int id { get; set; }
        public string? title { get; set; }
        public string? icon { get; set; }
        public List<string>? image { get; set; }

    }
}
