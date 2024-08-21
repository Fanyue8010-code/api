using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zym_api.Models
{
    public class BasicModel
    {
    }

    public class Category
    {
        public string ID { get; set; }
        public string Cate { get; set; }
        public List<Category> listCategory { get; set; } = new List<Category>();
    }

    public class GoodBasic
    {
        public string Action { get; set; }
        public Guid ID { get; set; } = Guid.NewGuid();
        public string GoodID { get; set; }
        public string CategoryID { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public string Barcode { get; set; }
        public string HasSubPack { get; set; } = "N";
        public string SubPackUnit { get; set; }
        public string SubPackBarcode { get; set; }
        public string SubPackQty { get; set; }
        public string Picture { get; set; }
    }

    public class MenuItems
    {
        public List<MenuList> menuList { get; set; } = new List<MenuList>();
    }

    public class MenuList
    {
        public string action { get; set; }
        public string title { get; set; }
        public List<Item> items { get; set; } = new List<Item>();
    }
    public class Item
    {
        public string title { get; set; }
        public string link { get; set; }

    }
}
