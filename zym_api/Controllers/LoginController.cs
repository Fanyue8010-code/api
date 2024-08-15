using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using zym_api.BLL;
using zym_api.Models;

namespace zym_api.Controllers
{
    public class LoginController : ApiController 
    {
        [HttpGet]
        public HttpResponseMessage GetOpenId()
        {
            return null;
        }
    }
}