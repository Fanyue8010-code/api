using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zym_api.Models
{
    public class AddressModel
    {
        public class AddressList
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Phone { get; set; }
            public string Region { get; set; }
            public string Address { get; set; }
            public string Default { get; set; }
        }
    }
}