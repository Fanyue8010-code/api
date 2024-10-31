using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using zym_api.BLL;
using zym_api.Models;
using static zym_api.Models.AddressModel;
using static zym_api.Models.GoodModel;
using System.Configuration;
using System.Data;
using zym_api.Helper;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Org.BouncyCastle.Utilities;
using System.Text.Json;
using zym_api.DAL;
using Org.BouncyCastle.Asn1;

namespace zym_api.Controllers
{
    public class GoodController : ApiController
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
        [HttpGet]
        public IHttpActionResult GetGoodCategory()
        {
            try
            {
                string errMsg = "";
                List<GoodCategory> list = GoodBLL.GetGoodCategory(out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(list);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetGoodBasic(string CategoryID, string SearchValue, string OpenId)
        {
            try
            {
                string errMsg = "";
                List<GoodCategory> list = GoodBLL.GetGoodBasic(CategoryID, SearchValue, out errMsg, OpenId);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(list);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult InsertCart(string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.InsertCart(OpenId, CategoryID, GoodBasicID, ShopGoodID, CategoryCheck, GoodCheck, GoodQty, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCart(string OpenId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GetCart(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCartGoodSum(string OpenId)
        {
            try
            {
                string errMsg = "";
                List<CartGoodSum> list = GoodBLL.GetCartGoodSum(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(list);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult MinusAmount(string OpenId, string CratId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.MinusAmount(OpenId, CratId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult PlusAmount(string OpenId, string CratId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.PlusAmount(OpenId, CratId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult UpdateCategoryCheck(string OpenId, string CategoryID, string CategoryCheck)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.UpdateCategoryCheck(OpenId, CategoryID, CategoryCheck, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult UpdateGoodCheck(string OpenId, string CartID, string GoodCheck)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.UpdateGoodCheck(OpenId, CartID, GoodCheck, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult UpdateCheckedAll(string OpenId, string checkedAll)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.UpdateCheckedAll(OpenId, checkedAll, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult DelCart(string OpenId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.DelCart(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GoodToCart(string CartID, string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GoodToCart(CartID, OpenId, CategoryID, GoodBasicID, ShopGoodID, CategoryCheck, GoodCheck, GoodQty, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCartGoodToPay(string OpenId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GetCartGoodToPay(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult InsertPayOrder(JObject jsonObj)
        {
            try
            {
                var jsonStr = JsonConvert.SerializeObject(jsonObj);
                var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                string Data = jsonParams.Orders.ToString().Replace("\n", "").Replace("\r", "");
                if (Data.StartsWith("{"))
                {
                    Data = "[" + Data + "]";
                }

                string OpenId = jsonParams.OpenId;
                string Name = jsonParams.Name;
                string Phone = jsonParams.Phone;
                string Region = jsonParams.Region;
                string Address = jsonParams.Address;
                string OrderNumber = jsonParams.OrderNumber;
                string Status = jsonParams.Status;
                string errMsg = "";
                string result = "";
                if (jsonParams.Type == "CartData")
                {
                    List<GoodsPayList> goodsPays = JsonConvert.DeserializeObject<List<GoodsPayList>>(Data);
                    result = GoodBLL.InsertPayOrder(goodsPays, OpenId, Name, Phone, Region, Address, out errMsg, OrderNumber, Status);
                }
                else
                {
                    List<GoodsPay> goodsPays = JsonConvert.DeserializeObject<List<GoodsPay>>(Data);
                    result = GoodBLL.InsertPayOrder(goodsPays, OpenId, Name, Phone, Region, Address, out errMsg, OrderNumber, Status);
                }
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetOrder(string OpenId, string ID = "", string menuTapCurrent = "")
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GetOrder(OpenId, ID, menuTapCurrent, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }

        [HttpGet]
        public async Task<object> CancelOrder(string OpenId, string OrderNumber)
        {
            try
            {


                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetNeedCancelOrder(OpenId, OrderNumber)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        string OrdersStatus = dr["Status"].ToString();
                        if (OrdersStatus != "待付款")
                        {
                            throw new Exception("此订单已经处于" + OrdersStatus + "请下拉刷新页面");
                        }
                    }
                }

                string errMsg = "";

                string MchId = GetMchId();
                string SerialNo = GetSerialNo();
                string appid = GetAppId();
                string method = "POST";

                HttpRequestMessage httpRequest = new HttpRequestMessage();
                //  string mchid = OrderNumber;
                string json = $"{{ \"mchid\": \"{MchId}\" }}";
                httpRequest.Method = HttpMethod.Post;
                httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
                httpRequest.Headers.Add("Accept", "*/*");
                httpRequest.Headers.Add("User-Agent", "dotnet/6.0");
                string uri = $"/v3/pay/transactions/out-trade-no/{OrderNumber}/close";
                var Subtimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                int length = 32; // 生成32个十六进制字符
                                 //  string randomHexString = GenerateRandomHexString(length);
                string nonce = Helper.Certificates.GenerateRandomHexString(length);
                string message = $"{method}\n{uri}\n{Subtimestamp}\n{nonce}\n{json}\n";

                string signature = Helper.Certificates.Sign(message);
                byte[] s = Encoding.UTF8.GetBytes(message);

                string authrizationValue = $"mchid=\"{MchId}\",nonce_str=\"{nonce}\",timestamp=\"{Subtimestamp}\",serial_no=\"{SerialNo}\",signature=\"{signature}\"";


                httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("WECHATPAY2-SHA256-RSA2048", authrizationValue);
                httpRequest.RequestUri = new Uri($"https://api.mch.weixin.qq.com/v3/pay/transactions/out-trade-no/{OrderNumber}/close");
                var httpClient = new HttpClient();
                await httpClient.SendAsync(httpRequest);

                string result = GoodBLL.CancelOrder(OpenId, OrderNumber, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult RefundOrder(string OpenId, string OrderNumber)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.RefundOrder(OpenId, OrderNumber, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult NoPayOrders(string OpenId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.NoPayOrders(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult ApplyRefund(string OpenId, string ID, string Phone, string Reason, string Type, string Price, string Remark)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.ApplyRefund(OpenId, ID, Phone, Reason, Type, Price, Remark, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public async Task<object> GetOrderPayStatus(string OpenId = "")
        {
            try
            {
                string OrderNumber = "";
                string result = "";
                string MchId = GetMchId();
                string SerialNo = GetSerialNo();
                string appid = GetAppId();
                string method = "GET";
                //获取订单状态为空，来单独调用查看订单状态，来更新订单
                using (DataTable dt = SQLHelper.ExecuteDataTable(PayDAL.GetOrderStatus(OpenId)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        OrderNumber = dr["OrderNumber"].ToString();
                        OpenId = dr["OpenID"].ToString();


                        HttpRequestMessage httpRequest = new HttpRequestMessage();
                        httpRequest.Method = HttpMethod.Get;
                        httpRequest.Headers.Add("Accept", "*/*");
                        httpRequest.Headers.Add("User-Agent", "dotnet/6.0");
                        string uri = $"/v3/pay/transactions/out-trade-no/{OrderNumber}?mchid={MchId}";
                        var Subtimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                        int length = 32; // 生成32个十六进制字符

                        string nonce = Helper.Certificates.GenerateRandomHexString(length);
                        string message = $"{method}\n{uri}\n{Subtimestamp}\n{nonce}\n\n";

                        string signature = Helper.Certificates.Sign(message);
                        byte[] s = Encoding.UTF8.GetBytes(message);

                        string authrizationValue = $"mchid=\"{MchId}\",nonce_str=\"{nonce}\",timestamp=\"{Subtimestamp}\",serial_no=\"{SerialNo}\",signature=\"{signature}\"";


                        httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("WECHATPAY2-SHA256-RSA2048", authrizationValue);
                        httpRequest.RequestUri = new Uri($"https://api.mch.weixin.qq.com/v3/pay/transactions/out-trade-no/{OrderNumber}?mchid={MchId}");
                        var httpClient = new HttpClient();
                        HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
                        string responseString = await response.Content.ReadAsStringAsync();
                        string tradestatePay = "";
                        string out_trade_no = "";
                        string transaction_id = "";
                        string trade_type = "";
                        using (JsonDocument doc = JsonDocument.Parse(responseString))
                        {
                            if (doc.RootElement.TryGetProperty("trade_state", out JsonElement tradestate))
                            {

                                tradestatePay = tradestate.ToString();

                            }
                            if (doc.RootElement.TryGetProperty("out_trade_no", out JsonElement outtradeno))
                            {

                                out_trade_no = outtradeno.ToString();

                            }
                            if (doc.RootElement.TryGetProperty("transaction_id", out JsonElement transactionid))
                            {

                                transaction_id = transactionid.ToString();

                            }
                            if (doc.RootElement.TryGetProperty("trade_type", out JsonElement tradetype))
                            {

                                trade_type = tradetype.ToString();

                            }

                        }
                        if (tradestatePay == "SUCCESS")
                        {
                            int UP = SQLHelper.ExecuteNonQuery(GoodDAL.NoPayOrderStatus(OpenId, OrderNumber, "待发货"));
                            int update = SQLHelper.ExecuteNonQuery(PayDAL.UpdateOrder(out_trade_no, transaction_id, trade_type, tradestatePay, OpenId));
                        }
                        if (tradestatePay == "NOTPAY")
                        {
                            int UP = SQLHelper.ExecuteNonQuery(GoodDAL.NoPayOrderStatus(OpenId, OrderNumber, "待付款"));
                            int update = SQLHelper.ExecuteNonQuery(PayDAL.UpdateOrder(out_trade_no, transaction_id, trade_type, tradestatePay, OpenId));
                        }
                        if (tradestatePay == "CLOSED")
                        {

                            int UP = SQLHelper.ExecuteNonQuery(GoodDAL.NoPayOrderStatus(OpenId, OrderNumber, "已取消"));
                            int update = SQLHelper.ExecuteNonQuery(PayDAL.UpdateOrder(out_trade_no, transaction_id, trade_type, tradestatePay, OpenId));
                        }
                    }
                    result = "OK";
                }

                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }

        [HttpGet]
        public HttpResponseMessage GetOrderList(string orderNo, string status, string prepare, string start, string end)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(GoodBLL.GetOrderList(orderNo, status, prepare, start, end));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage ChgPrepare(string orderNo, string id)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(GoodBLL.ChgPrepare(orderNo, id));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetFee()
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(GoodBLL.GetFee());
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage ChgFee(string fee, string free)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(GoodBLL.ChgFee(fee, free));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpPost]
        public HttpResponseMessage Shipping(JObject json)
        {
            string strJson = "";
            try
            {
                var jsonStr = JsonConvert.SerializeObject(json);
                var jsonPara = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                JArray arrOrder = jsonPara.OrderNumber;
                //先通过api查询订单是否发货
                foreach (var obj in arrOrder)
                {
                    string strOrder = obj.ToString();
                    GoodBLL.GetOrderStatus(strOrder);
                }
                //没问题，发货
                foreach (var obj in arrOrder)
                {
                    string strOrder = obj.ToString();
                    GoodBLL.Shipping(strOrder);
                }

                //strJson = JsonConvert.SerializeObject(jsonObj);
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }
    }
}