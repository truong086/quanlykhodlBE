﻿namespace quanlykhodl.ViewModel
{
    public class productArea
    {
        public int Id { get; set; }
        public int quantity {  get; set; }
        public int totalLocation {  get; set; }
        public int totalLocationEmpty {  get; set; }
        public int totalLocatiEmpty {  get; set; }
        public List<productLocationArea>? productLocationAreas { get; set; }
        public List<productLocationArea>? productPlans { get; set; }
        public Dictionary<int, int>? locationTotal {  get; set; }

    }

    public class productLocationArea
    {
        public int Id { get; set; }
        public int Id_product { get; set; }
        public int Id_plan { get; set; }
        public string? name { get; set; }
        public string? image { get; set; }
        public int? location { get; set; }
        public int quantity { get; set; }
    }
}
