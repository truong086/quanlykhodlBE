﻿using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class productImportform : BaseEntity
	{
		public int? importform { get; set; }
		public int? product { get; set; }
		public int quantity { get; set; }
        public int? supplier { get; set; }
		public int? location { get; set; }
		public int? area_id { get; set; }
		public bool isAction { get; set; }
		public string? code { get; set; }
		public Area? area { get; set; }
        public Supplier? supplier_id { get; set; }
        public Importform? importform_id1 { get; set; }
		public product? products { get; set; }
	}
}
