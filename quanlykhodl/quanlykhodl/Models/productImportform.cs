﻿using quanlykhodl.Common;

namespace quanlykhodl.Models
{
	public class productImportform : BaseEntity
	{
		public int? importform { get; set; }
		public int? product { get; set; }
		public Importform? importform_id1 { get; set; }
		public product? products { get; set; }
	}
}