using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Management;

namespace zym_api.Helper
{
    public class ControllerFeedback
    {
        public static HttpResponseMessage ExJson(Exception ex)
        {
            HttpResponseMessage msg = new HttpResponseMessage
            {
                Content = new StringContent(JsonConvert.SerializeObject(new { success = 0, errmsg = ex.Message }), Encoding.GetEncoding("UTF-8"), "application/json")
            };
            return msg;
        }

        public static HttpResponseMessage OKJson(string json)
        {
            HttpResponseMessage msg = new HttpResponseMessage 
            { 
                Content = new StringContent(JsonConvert.SerializeObject(new { success = 1, msg = JsonConvert.DeserializeObject(json) }), Encoding.GetEncoding("UTF-8"), "application/json") 
            };
            return msg;
        }
    }
}