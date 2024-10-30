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
using zym_api.Models;
using static zym_api.Helper.Helper;

namespace zym_api.Controllers
{
    public class SysController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetOrder()
        {
            string strJson = "";
            try
            {
                OrderList order = new OrderList();
                order.pay_time_range.begin_time = 1729950058;
                order.pay_time_range.end_time = 1729970658;
                order.order_state = 2;
                order.page_size = 2;
                var j = JsonHelper.Post("https://api.weixin.qq.com/wxa/sec/order/get_order_list?access_token=85_m-GjfsDNDHb7aajVy7J2V0CKFJhdbqzg7Tmv6wb9Hv4di_Bl6AtLIs2If4UC86MIiJs2mp8fAq9bVnjGcxoumGYflEFQGK51tyk7KcftiTJ_cNKL7TgxCuPYs-YMLSbAJAYXE",
                    JsonConvert.SerializeObject(order));
                strJson = JsonConvert.SerializeObject(j);
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

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

        [HttpGet]
        public void Base64()
        {
            SQLHelper.DB();
            string originalString = "ZYM";
            string encodedString = originalString;

            // 第一次Base64编码
            encodedString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(encodedString));

            // 第二次Base64编码
            encodedString = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(encodedString));

        }

    }
}