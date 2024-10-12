using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using zym_api.Models;
using System.Net.Http;
using zym_api.Helper;
using System.Data;
using System.Security.Cryptography;
using System.Web.Services.Description;
using System.IO;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Microsoft.CodeAnalysis;
using static zym_api.Models.Refund;
using System.Linq.Expressions;
using Org.BouncyCastle.Asn1.Ocsp;


namespace zym_api.Controllers
{
    public class SubmitOrderController : ApiController
    {
        private IHttpActionResult DoOKReturn(object returnObj)
        {
            return DoReturn("0", returnObj, null);
        }
        private IHttpActionResult DoErrorReturn(string errorMsg)
        {
            return DoReturn(null, null, errorMsg);
        }
        private IHttpActionResult DoReturn(string code, object returnObj, string message)
        {
            QueryReturnModel qre = new QueryReturnModel();
            qre.code = code;
            qre.data = returnObj;
            qre.message = message;
            return Ok(qre);
        }

        public string GetMchId()
        {
            DataTable dt = SQLHelper.ExecuteDataTable("select Config2 from [dbo].[Config] where Config1 = 'mchId'");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Config2"].ToString();
            }
            return null; // 或抛出异常，视具体需求而定
        }
        public string GetSerialNo()
        {
            DataTable dt = SQLHelper.ExecuteDataTable("select Config2 from [dbo].[Config] where Config1 = 'serialNo'");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Config2"].ToString();
            }
            return null; // 或抛出异常，视具体需求而定
        }
        public string GetAppId()
        {
            DataTable dt = SQLHelper.ExecuteDataTable("select Config2 from [dbo].[Config] where Config1 = 'pay_appid'");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Config2"].ToString();
            }
            return null; // 或抛出异常，视具体需求而定
        }


        [HttpPost]
        // public async Task<WeiXinPayrequestPayment> SubmitOrderAsync([FromBody] WeiXinPayOrderRequestDTO request)
        public async Task<WeiXinPayrequestPayment> SubmitOrderAsync(JObject jsonObj)
        {

            try
            {
                string MchId = GetMchId();
                string SerialNo = GetSerialNo();
                string appid = GetAppId();
                string method = "POST";
                var guid = Guid.NewGuid().ToString("N");
                var jsonStr = JsonConvert.SerializeObject(jsonObj);
                var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                string Data = jsonParams.detail.ToString().Replace("\n", "").Replace("\r", "");
                if (Data.StartsWith("{"))
                {
                    Data = "[" + Data + "]";
                }


                var jsonArray = new JArray();
                if (Data.StartsWith("["))
                {
                    jsonArray = JArray.Parse(Data);
                    // 处理 jsonArray
                }
                WeiXinPayOrderRequestDTO request = new WeiXinPayOrderRequestDTO();


                // 循环遍历每一项
                foreach (var category in jsonArray)
                {
                    //string categoryId = category["CategoryID"].ToString();
                    // string categoryName = category["Category"].ToString();

                    // Console.WriteLine($"分类ID: {categoryId}, 分类名称: {categoryName}");

                    // 获取并循环遍历 merchandises 数组
                    var merchandises = category["merchandises"] == null ? category["Merchandises"] : category["merchandises"];
                    foreach (var merchandise in merchandises)
                    {
                        // 假设每个 merchandise 里有 name 和 price 属性

                        string name = merchandise["GoodName"]?.ToString();  // 替换为实际的键名

                        int quantity = merchandise["GoodQty"]?.Value<int>() ?? 0;

                        decimal priceDecimal = merchandise["Price"]?.Value<decimal>() ?? 0;
                        int priceInCents = Convert.ToInt32(priceDecimal * 100);
                        Guid goodid = Guid.Parse(merchandise["ShopGoodID"].ToString());
                        string guidAs32BitString = goodid.ToString("N");
                        var refundDetail = new DetailModel
                        {
                            merchant_goods_id = guidAs32BitString,  // 替换为实际的键名
                            goods_name = name,
                            quantity = quantity,
                            unit_price = priceInCents
                        };

                        // 将 refundDetail 添加到 goods_detail 列表中
                        request.detail.goods_detail.Add(refundDetail);
                    }
                }


                request.payer.openid = jsonParams.payer;
                request.description = jsonParams.Description;
                request.amount.total = jsonParams.total;
                request.out_trade_no = jsonParams.out_trade_no == null ? "" : jsonParams.out_trade_no;
                request.appid = appid;
                request.mchid = MchId;
                request.out_trade_no = request.out_trade_no == "" ? guid.Substring(0, 32) : request.out_trade_no;
                request.notify_url = "https://www.zymlsd.com/zym_api/api/payment/payNotify";

                HttpRequestMessage httpRequest = new HttpRequestMessage();
                string json = JsonConvert.SerializeObject(request);
                httpRequest.Method = HttpMethod.Post;
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                httpRequest.Headers.Add("Accept", "*/*");
                httpRequest.Headers.Add("User-Agent", "dotnet/6.0");
                string uri = "/v3/pay/transactions/jsapi";
                var Subtimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                int length = 32; // 生成32个十六进制字符
                                 //  string randomHexString = GenerateRandomHexString(length);
                string nonce = Helper.Certificates.GenerateRandomHexString(length);
                string message = $"{method}\n{uri}\n{Subtimestamp}\n{nonce}\n{json}\n";

                string signature = Helper.Certificates.Sign(message);
                byte[] s = Encoding.UTF8.GetBytes(message);

                string authrizationValue = $"mchid=\"{MchId}\",nonce_str=\"{nonce}\",timestamp=\"{Subtimestamp}\",serial_no=\"{SerialNo}\",signature=\"{signature}\"";


                httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("WECHATPAY2-SHA256-RSA2048", authrizationValue);
                httpRequest.RequestUri = new Uri("https://api.mch.weixin.qq.com/v3/pay/transactions/jsapi");
                var httpClient = new HttpClient();
                HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
                string responseString = await response.Content.ReadAsStringAsync();
                Helper.Log.WriteLog($"Response: {responseString}");
                var orderResult = Newtonsoft.Json.JsonConvert.DeserializeObject<WeiXinPayOrderResponseDTO>(responseString);
                string prepay_id = orderResult.prepay_id;
                DateTime utcNow = DateTime.UtcNow;

                // 将 UTC 时间转换为东八区时间
                DateTime beijingTime = utcNow.AddHours(8);

                // 获取自1970年1月1日以来的总秒数
                long time = (long)(beijingTime - new DateTime(1970, 1, 1)).TotalSeconds;
                string timestamp = time.ToString();
                // int length = 32; // 生成32个十六进制字符
                //  string randomHexString = GenerateRandomHexString(length);
                string nonceStr = Helper.Certificates.GenerateRandomHexString(length);
                string package = $"prepay_id={prepay_id}";
                string dataToSign = appid + "\n" + timestamp + "\n" + nonceStr + "\n" + package + "\n";
                string sign = "";
                // 创建RSA加密服务提供程序实例
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    // 加载私钥
                    sign = Helper.Certificates.Sign(dataToSign);
                }
                int pay = request.amount.total;
                // 创建 WeiXinPayrequestPayment 对象
                WeiXinPayrequestPayment PayRequest = new WeiXinPayrequestPayment();
                PayRequest.nonceStr = nonceStr;
                PayRequest.package = package;
                PayRequest.paySign = sign;
                PayRequest.timeStamp = timestamp;
                PayRequest.out_trade_no = request.out_trade_no;
                //return new WeiXinPayrequestPayment
                //{
                //    nonceStr = nonceStr,
                //    package = package,
                //    paySign = sign,
                //    timeStamp = timestamp,
                //    out_trade_no = request.out_trade_no
                //};
                return PayRequest;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
