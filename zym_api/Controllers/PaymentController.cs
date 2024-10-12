using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Modes;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Tls;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Xml;
using zym_api.Helper;
using zym_api.Models;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.X509;
using System.Security.Cryptography.X509Certificates;
using zym_api.DAL;
using System.Net;
using System.Web.Http.Results;
using static System.Net.WebRequestMethods;
using Org.BouncyCastle.Asn1.X9;

namespace zym_api.Controllers
{


    public class PaymentController : ApiController
    {
        //private IHttpActionResult DoOKReturn(object returnObj)
        //{
        //    return DoReturn("0", returnObj, null);
        //}
        //private IHttpActionResult DoErrorReturn(string errorMsg)
        //{
        //    return DoReturn(null, null, errorMsg);
        //}
        //private IHttpActionResult DoReturn(string code, object returnObj, string message)
        //{
        //    QueryReturnModel qre = new QueryReturnModel();
        //    qre.code = code;
        //    qre.data = returnObj;
        //    qre.message = message;
        //    return Ok(qre);
        //}

        //[HttpGet]
        //public IHttpActionResult UserInfo()
        //{
        //    Helper.Log.WriteLog("支付成功");
        //    return DoOKReturn("ok");
        //}
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
        public class YourRequestModel
        {
            public string id { get; set; }
            public string create_time { get; set; }
            public string event_type { get; set; }
            public string resource_type { get; set; }
            public ResourceType resource { get; set; } // 这里定义为对象
            public string summary { get; set; }
        }
        public class ResourceType
        {
            // 根据实际 JSON 结构定义属性
            public string original_type { get; set; }
            public string algorithm { get; set; }
            public string ciphertext { get; set; }
            public string associated_data { get; set; }
            public string nonce { get; set; }

        }
        public class YourResourceType
        {
            // 根据实际 JSON 结构定义属性
            public string appid { get; set; }
            public string mchid { get; set; }
            public string out_trade_no { get; set; }
            public string transaction_id { get; set; }
            public string trade_type { get; set; }
            public string trade_state { get; set; }

            public string trade_state_desc { get; set; }
            public string bank_type { get; set; }

            public string success_time { get; set; }

            public WeiXinPayOrderPayer payer { get; set; }

            public WeiXinPayOrderAmout amount { get; set; }

        }

        // 添加其他必要的属性

        [HttpPost]
        [Route("api/payment/payNotify")]
        public async Task<IHttpActionResult> PayNotify()
        {
            try
            {

                var body = await Request.Content.ReadAsStringAsync();
                //  Helper.Log.WriteLog("Request.Headers：" + Request.Headers.ToString());

                var jsonData = JsonConvert.DeserializeObject<YourRequestModel>(body);


                // 获取请求头
                //微信发来的签名
                var signature = Request.Headers.GetValues("wechatpay-signature").FirstOrDefault();
                //Helper.Log.WriteLog("wechatpay-signature：" + signature);
                //微信平台证书序列号
                var serial = Request.Headers.GetValues("wechatpay-serial").FirstOrDefault();
                // Helper.Log.WriteLog("wechatpay-serial：" + serial);
                //应答随机串
                var nonce = Request.Headers.GetValues("wechatpay-nonce").FirstOrDefault();
                //  Helper.Log.WriteLog("wechatpay-nonce：" + nonce);
                //应答时间戳
                var timestamp = Request.Headers.GetValues("wechatpay-timestamp").FirstOrDefault();
                // Helper.Log.WriteLog("wechatpay-timestamp：" + timestamp);
                //VerifySign验签
                //  bool isVerified = false;
                //   bool isVerified = await VerifySign(timestamp, nonce, body, serial, signature);
                //string json = JsonConvert.SerializeObject(body);
                //  Helper.Log.WriteLog("json：" + body);
                if (timestamp != null && nonce != null && serial != null && signature != null)
                {
                    // 验证签名
                    bool isVerified = await VerifySign(timestamp, nonce, body, serial, signature);
                    Helper.Log.WriteLog("isVerified：" + isVerified);
                    if (isVerified && jsonData.event_type == "TRANSACTION.SUCCESS" && !string.IsNullOrEmpty(body))
                    {

                        var resource = jsonData.resource; // 不再是数组，直接取出对象
                        var resultStr = Decrypt(resource.ciphertext, resource.associated_data, resource.nonce);

                        var jData = JsonConvert.DeserializeObject<YourResourceType>(resultStr);
                        Helper.Log.WriteLog("jData：" + jData);
                        //  await Task.Delay(TimeSpan.FromSeconds(2));

                        int update = SQLHelper.ExecuteNonQuery(PayDAL.UpdateOrder(jData.out_trade_no, jData.transaction_id, jData.trade_type, jData.trade_state, jData.payer.openid));
                        // Helper.Log.WriteLog("insert：" + update);

                        return Ok(new { code = "SUCCESS", message = "Callback processed successfully" });
                    }
                    else
                    {
                        return Ok(new { code = "FAIL", message = "Payment failed or incomplete" });
                    }
                }
                else
                {

                    //  return StatusCode(500, "Error processing callback is null");
                    // var errorResponse = new { error = "HTTP error processing callback: " + httpEx.Message };
                    var errorResponse = new { error = "Error processing callback" };
                    return Content(HttpStatusCode.InternalServerError, errorResponse);
                }

                //  Helper.Log.WriteLog("isVerified: {isVerified}"+isVerified);


            }
            catch (Exception ex)
            {
                Helper.Log.WriteLog(ex + "Callback Error");
                var errorResponse = new { error = "Error processing callback: " + ex.Message };
                return Content(HttpStatusCode.InternalServerError, errorResponse);
            }

            //  return Ok("FAIL");
        }
        private async Task<bool> VerifySign(string timestamp, string nonce, string body, string serial, string signature)
        {
            string publicKey = await FetchWechatPayPublicKey(serial);
            Helper.Log.WriteLog("publicKey：" + publicKey);
            string bodyStr = body; // body 已经是字符串

            string data = $"{timestamp}\n{nonce}\n{bodyStr}\n";
            // 将 PEM 字符串转换为字节数组
            byte[] certBytes = Encoding.ASCII.GetBytes(publicKey);

            // 创建 X509Certificate2 实例
            using (var wechatCert = new X509Certificate2(certBytes))
            {
                // 构建待验证的数据字符串
                string combinedString = $"{timestamp}\n{nonce}\n{body}\n";
                byte[] buff = Encoding.UTF8.GetBytes(combinedString);
                byte[] signatureBytes = Convert.FromBase64String(signature);
                Helper.Log.WriteLog($"Combined String: {combinedString}");
                Helper.Log.WriteLog($"Signature: {signature}");
                // 获取 RSA 公钥
                using (var rsa = wechatCert.GetRSAPublicKey())
                {
                    if (rsa == null)
                        throw new InvalidOperationException("无法获取RSA公钥。");

                    // 验证数据签名
                    bool isValid = rsa.VerifyData(buff, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
                    return isValid;
                }
            }
        }
        public string GetApiV3()
        {
            DataTable dt = SQLHelper.ExecuteDataTable("select Config2 from [dbo].[Config] where Config1 = 'pay_api'");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Config2"].ToString();
            }
            string df = dt.Rows[0]["Config2"].ToString();

            return null; // 或抛出异常，视具体需求而定
        }
        public string Decrypt(string ciphertext, string associatedData, string nonce)
        {
            // 将 Base64 编码的 APIv3 密钥解码为字节数组
            string ApiV3 = GetApiV3();

            GcmBlockCipher gcmBlockCipher = new GcmBlockCipher(new AesEngine());
            AeadParameters aeadParameters = new AeadParameters(
                new KeyParameter(Encoding.UTF8.GetBytes(ApiV3)),
                128,
                Encoding.UTF8.GetBytes(nonce),
                Encoding.UTF8.GetBytes(associatedData));
            //  Helper.Log.WriteLog("密钥字节长度: " + aeadParameters.Key.KeyLength); // 应该输出 32
            gcmBlockCipher.Init(false, aeadParameters);
            byte[] data = Convert.FromBase64String(ciphertext);
            byte[] plaintext = new byte[gcmBlockCipher.GetOutputSize(data.Length)];
            int length = gcmBlockCipher.ProcessBytes(data, 0, data.Length, plaintext, 0);
            gcmBlockCipher.DoFinal(plaintext, length);
            return Encoding.UTF8.GetString(plaintext);
        }


        public async Task<string> FetchWechatPayPublicKey(string serial)
        {
            using (var httpClient1 = new HttpClient())
            {
                string method1 = "GET";
                // 示例 URL，请替换为真实的微信支付证书接口
                string url = $"https://api.mch.weixin.qq.com/v3/certificates";
                HttpRequestMessage httpRequest1 = new HttpRequestMessage();
                httpRequest1.Method = HttpMethod.Get;
                httpRequest1.Headers.Add("Accept", "*/*");
                httpRequest1.Headers.Add("User-Agent", "dotnet/6.0");
                string uri1 = "/v3/certificates";
                var Subtimestamp1 = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                int length1 = 32; // 生成32个十六进制字符
                                  //  string randomHexString = GenerateRandomHexString(length);
                string nonce1 = Helper.Certificates.GenerateRandomHexString(length1);
                string message1 = $"{method1}\n{uri1}\n{Subtimestamp1}\n{nonce1}\n\n";

                string signature1 = Helper.Certificates.Sign(message1);
                byte[] s1 = Encoding.UTF8.GetBytes(message1);
                string MchId1 = GetMchId();
                string SerialNo1 = GetSerialNo();
                string authrizationValue1 = $"mchid=\"{MchId1}\",nonce_str=\"{nonce1}\",timestamp=\"{Subtimestamp1}\",serial_no=\"{SerialNo1}\",signature=\"{signature1}\"";

                httpRequest1.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("WECHATPAY2-SHA256-RSA2048", authrizationValue1);
                httpRequest1.RequestUri = new Uri("https://api.mch.weixin.qq.com/v3/certificates");
                //httpClient = new HttpClient();
                HttpResponseMessage response1 = await httpClient1.SendAsync(httpRequest1);
                string responseString1 = await response1.Content.ReadAsStringAsync();
                //  Helper.Log.WriteLog($"Response: {responseString1}");

                dynamic data = Newtonsoft.Json.JsonConvert.DeserializeObject(responseString1);
                foreach (var item in data.data)
                {
                    if (item.serial_no.ToString() == serial)
                    {
                        // Helper.Log.WriteLog(item.serial_no.ToString());
                        //    Helper.Log.WriteLog("encrypt_certificate: " + item.encrypt_certificate.ToString());
                        //   Helper.Log.WriteLog("ciphertext：" + item.encrypt_certificate.ciphertext.ToString());
                        //   Helper.Log.WriteLog("nonce：" + item.encrypt_certificate.nonce.ToString());
                        //     Helper.Log.WriteLog("associated_data：" + item.encrypt_certificate.associated_data.ToString());
                        string certificate = Decrypt(item.encrypt_certificate.ciphertext.ToString(), item.encrypt_certificate.associated_data.ToString(), item.encrypt_certificate.nonce.ToString());
                        //  Helper.Log.WriteLog("certificate：" + certificate);
                        return certificate; // 返回证书内容
                    }
                }

                throw new Exception("Public key not found");
            }
        }
    }
}