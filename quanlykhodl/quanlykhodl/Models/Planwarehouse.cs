﻿using quanlykhodl.Common;

namespace quanlykhodl.Models
{
    public class Planwarehouse : BaseEntity
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public string? status { get; set; }
        public bool isConfirmation { get; set; }
        public bool isConsent { get; set; }
        public int? Receiver { get; set; }
        public int? floorOld { get; set; }
        public int? warehouse { get; set; }
        public int? area { get; set; }
        public int? floor { get; set; }
        public accounts? Receiver_id { get; set; }
        public Warehouse? warehouse_id { get; set; }
        public Shelf? areaid { get; set; }
        public Floor? floor_id { get; set; }
        public virtual ICollection<Warehousetransferstatus>? warehousetransferstatuses { get; set; }
    }
}
