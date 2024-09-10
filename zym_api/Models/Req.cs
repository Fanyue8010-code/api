using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace zym_api.Models
{
    public class Req
    {
        public int? Rows { get; set; }
        public DataTable ReqList { get; set; }
    }
}