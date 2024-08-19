using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zym_api.Models
{
    public class QueryReturnModel
    {
        public string code { get; set; }
        public object data { get; set; }
        public string message { get; set; }

    }

}