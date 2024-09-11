using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using JWT;
using JWT.Algorithms;
using JWT.Serializers;
using Newtonsoft.Json;
using zym_api.BLL;
using zym_api.Helper;
using static zym_api.Helper.Helper;

namespace zym_api.Controllers
{
    public class SysController : ApiController
    {
        // GET: Sys
        [HttpGet]
        public string GetToken()
        {
            var payload = new Dictionary<string, object> 
            {
                { "UserName", "A"},
                { "CurrentTime", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},
                { "ExpireTime", DateTime.Now.AddDays(30).ToString("yyyy-MM-dd HH:mm:ss") }
            };
            IJwtAlgorithm alo = new HMACSHA256Algorithm();
            IJsonSerializer serial = new JsonNetSerializer();
            //IDateTimeProvider provider = new UtcDateTimeProvider();
            //IJwtValidator vali = new JwtValidator(serial, provider);
            IBase64UrlEncoder urlEncode = new JwtBase64UrlEncoder();
            IJwtEncoder encoder = new JwtEncoder(alo, serial, urlEncode);
            string strSecret = "GQDsjcKsx0NHJPOuXOYg5MbeG1XT0uFiwDVvVHrk";
            string strToken = encoder.Encode(payload, strSecret);
            return "Bearer " + strToken;
        }

        [HttpGet]
        public HttpResponseMessage Hello()
        {            
            string str = "INSERT INTO GOODCATEGORY(ID, CATEGORY) VALUES ('" + Guid.NewGuid() + "', '水果')";
            SQLHelper.ExecuteNonQuery(str);
            DataTable dt = new DataTable();
            dt.Columns.Add("A");
            DataRow dr = dt.NewRow();
            dr["A"] = "111";
            dt.Rows.Add(dr);
            string strJson = JsonConvert.SerializeObject(dt);
            HttpResponseMessage msg = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return msg;

        }

    }
}