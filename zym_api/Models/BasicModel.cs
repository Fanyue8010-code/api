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
        public Guid ID { get; set; } = Guid.NewGuid();
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
}