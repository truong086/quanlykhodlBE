﻿namespace quanlykhodl.ViewModel
{
    public class DeliverynoteDTO
    {
        public string? title { get; set; }
        public string? description { get; set; }
        public bool isRetailcustomers { get; set; }
        public List<productDeliverynoteDTO>? products { get; set; }
        public int? RetailcustomersOld { get; set; }
        public string? name { get; set; }
        public string? address { get; set; }
        public string? phone { get; set; }
        public string? email { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public double total { get; set; }
        public string? DeliveryAddress { get; set; }
        public string? code { get; set; }
        public int? Tax { get; set; }
        public bool? isPercentage { get; set; }
    }

    public class productDeliverynoteDTO
    {
        public int id_product { get; set; }
        public int quantity { get; set; }
        public int? location { get; set; }
        public int? area { get; set; }
    }

    public class UploadproductDeliverynoteDTO
    {
        public int? productDelivenote_id { get; set; }
        public int? id_product { get; set; }
        public int? quantity { get; set; }
        public int? location { get; set; }
        public int? area { get; set; }
    }

    public class uploadDataLocationArea
    {
        public string? code { get; set;}
        public int? id { get; set;}
        public List<UploadproductDeliverynoteDTO>? products { get; set; }
    }

    public class updatePack
    {
        public string? code { get; set; }
        public int? id { get; set; }
    }
}
