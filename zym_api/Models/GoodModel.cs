using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zym_api.Models
{
    public class GoodModel
    {
        public class GoodCategory
        {
            public string Id { get; set; }
            public string GoodBasicID { get; set; }
            public string CategoryID { get; set; }
            public string ShopGoodID { get; set; }
            public string Category { get; set; }
            public string GoodName { get; set; }
            public string Picture { get; set; }
            public decimal Price { get; set; }
            public string GoodQty { get; set; }
            public string CartID { get; set; }
        }
        public class Cart
        {
            public string CategoryCheck { get; set; }
            public string GoodCheck { get; set; }
            public string CartID { get; set; }
            public string GoodBasicID { get; set; }
            public string CategoryID { get; set; }
            public string ShopGoodID { get; set; }
            public string Category { get; set; }
            public string GoodName { get; set; }
            public string Picture { get; set; }
            public decimal Price { get; set; }
            public string GoodQty { get; set; }
        }
        public class CartGoodSum
        {
            public string Total { get; set; }
            public string TotalCount { get; set; }

        }
        public class GoodsPayList
        {
            public string CategoryID { get; set; }
            public string Category { get; set; }
            public string CategoryCheck { get; set; }

            public  List<GoodsPay> merchandises{ get; set; }
          
        }
        public class GoodsPay
        {   public string ID { get; set; }
           public string  OpenId { get; set; }
             public string CartID { get; set; }
              public string GoodCheck { get; set; }
            public string OrderNumber { get; set; }
            public string GoodBasicID { get; set; }
            public string CategoryID { get; set; }
            public string ShopGoodID { get; set; }
            public string Category { get; set; }
            public string GoodName { get; set; }
            public string Picture { get; set; }
            public decimal Price { get; set; }
            public string GoodQty { get; set; }
            public string CreateTime { get; set; }
            public string PayTime { get; set; }
            public string ShipDate { get; set; }
            public string  CompletionTime { get; set; }
            public string  Name { get; set; }
            public string  Phone { get; set; }
            public string Region { get; set; }
            public string Address { get; set; }
            public string Status { get; set; }
        }
    }
}