namespace quanlykhodl.ViewModel
{
    public class AreagetAllData
    {
        public int? id {  get; set; }
        public string? name { get; set; }
        public List<dataListLineBtArea>? data { get; set; }
    }

    public class dataListLineBtArea
    {
        public int? id { get; set; }
        public string? name { get; set; }
        public int quantityshelf { get; set; }
        public List<ShelfGetAll>? shelfGetAlls { get; set; }
    }
}
