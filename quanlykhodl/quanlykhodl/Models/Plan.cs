using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class Plan : BaseEntity
	{
		public string? title { get; set; }
		public string? description { get; set; }
		public string? status { get; set; }
		public int? localtionnew { get; set; }
        public bool isconfirmation { get; set; }
        public bool isconsent { get; set; }
        public bool iswarehourse { get; set; }
        public int? productlocation_map { get; set; }
		public int? Receiver { get; set; }
        public int? localtionold { get; set; }
        public int? warehouseold { get; set; }
        public int? shelfOld { get; set; }
        public int? areaold { get; set; }
        public int? floorold { get; set; }
        public int? warehouse { get; set; }
		public int? shelf { get; set; }
		public int? floor { get; set; }
		public int? area { get; set; }
		public productlocation? productidlocation { get; set; }
		public accounts? Receiver_id { get; set; }
		public Warehouse? warehouse_id { get; set; }
		public Shelf? shelfid { get; set; }
		public Floor? floor_id { get; set; }
		public areas? area_id { get; set; }
		public virtual ICollection<Warehousetransferstatus>? warehousetransferstatuses { get; set; }
		public virtual ICollection<producthisstorylocation>? Producthisstorylocations { get; set; }
	}
}
