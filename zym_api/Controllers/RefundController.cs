using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Asn1.Ocsp;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using zym_api.DAL;
using zym_api.Helper;
using zym_api.Models;
using static zym_api.Models.Refund;

namespace zym_api.Controllers
{
	public class RefundController : ApiController
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
		public async Task<IHttpActionResult> RefundOrderAsync(JObject jsonObj)
		{
			string errMsg = "";
			//string payer_refund = (a / 100.0).ToString();
			try
			{

				ApplyAmount request = new ApplyAmount();
				var jsonStr = JsonConvert.SerializeObject(jsonObj);
				var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
				string id = jsonParams.id;
				string out_trade_no = jsonParams.out_trade_no;
				request.out_trade_no = out_trade_no;
                string reason = jsonParams.reason;
                request.reason = reason;
				if (reason == "用户提交退款")
				{

                    if (!string.IsNullOrEmpty(id) && id != "undefined")
                    {
                        using (DataTable dt = SQLHelper.ExecuteDataTable(PayDAL.GetOrdersStatus(id)))
                        {
                            foreach (DataRow dr in dt.Rows)
                            {
                                string OrdersStatus = dr["Status"].ToString();
                                if (OrdersStatus != "待发货")
                                {
                                    throw new Exception("此订单已经处于" + OrdersStatus + "请下拉刷新页面");
                                }
                            }
                        }
                    }
                    else if (!string.IsNullOrEmpty(request.out_trade_no))
                    {
                        using (DataTable dt = SQLHelper.ExecuteDataTable(PayDAL.GetOrdersNumber(request.out_trade_no)))
                        {
                            if (dt.Rows.Count <= 0)
                            {
                                throw new Exception("此订单已经不处于待发货，无权退款，请下拉刷新页面");
                            }
                        }
                    }
                }

				string MchId = GetMchId();
				string SerialNo = GetSerialNo();
				string appid = GetAppId();
				string method = "POST";
				var guid = Guid.NewGuid().ToString("N");
				string json = "";

				// string out_refund_no = jsonParams.out_refund_no;
				HttpRequestMessage httpRequest = new HttpRequestMessage();
				// 将 goods_detail 转换为 JSON 字符串并存储
				request.out_refund_no = request.out_refund_no == "" ? guid.Substring(0, 32) : request.out_refund_no;

				//  request.merchant_goods_id = jsonParams.merchant_goods_id;
				request.amount.total = Convert.ToInt32(jsonParams.total);
				request.amount.refund = Convert.ToInt32(jsonParams.refund);
				//string reason = jsonParams.reason;
			//	request.reason = reason;


				var goodsDetail = new RefundsDetailModel();
				if (!string.IsNullOrEmpty(id) && id != "undefined")
				{
					string GoodNum = jsonParams.merchant_goods_id;
					Guid goodid = Guid.Parse(GoodNum);
					string guidAs32BitString = goodid.ToString("N");
					goodsDetail = new RefundsDetailModel
					{
						merchant_goods_id = guidAs32BitString,
						goods_name = jsonParams.goodname,
						unit_price = Convert.ToInt32(jsonParams.unit_price),
						refund_amount = Convert.ToInt32(jsonParams.refund_amount),
						refund_quantity = Convert.ToInt32(jsonParams.refund_quantity)
					};
					request.goods_detail.Add(goodsDetail);
					json = JsonConvert.SerializeObject(request);
				}
				else
				{
					ApplyRefund applyOrder = new ApplyRefund();

					applyOrder.out_trade_no = jsonParams.out_trade_no;
					applyOrder.out_refund_no = request.out_refund_no;
					applyOrder.amount.total = Convert.ToInt32(jsonParams.total);
					applyOrder.amount.refund = Convert.ToInt32(jsonParams.refund);
					reason = jsonParams.reason;
					applyOrder.reason = reason;
					json = JsonConvert.SerializeObject(applyOrder);
				}

				//	string json = JsonConvert.SerializeObject(request);
				httpRequest.Method = HttpMethod.Post;
				httpRequest.Content = new StringContent(json, Encoding.UTF8, "application/json");
				httpRequest.Headers.Add("Accept", "*/*");
				httpRequest.Headers.Add("User-Agent", "dotnet/6.0");
				string uri = "/v3/refund/domestic/refunds";
				var Subtimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
				int length = 32; // 生成32个十六进制字符
								 //  string randomHexString = GenerateRandomHexString(length);
				string nonce = Helper.Certificates.GenerateRandomHexString(length);
				string message = $"{method}\n{uri}\n{Subtimestamp}\n{nonce}\n{json}\n";

				string signature = Helper.Certificates.Sign(message);
				byte[] s = Encoding.UTF8.GetBytes(message);

				string authrizationValue = $"mchid=\"{MchId}\",nonce_str=\"{nonce}\",timestamp=\"{Subtimestamp}\",serial_no=\"{SerialNo}\",signature=\"{signature}\"";


				httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("WECHATPAY2-SHA256-RSA2048", authrizationValue);
				httpRequest.RequestUri = new Uri("https://api.mch.weixin.qq.com/v3/refund/domestic/refunds");
				var httpClient = new HttpClient();
				HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
				string responseString = await response.Content.ReadAsStringAsync();
				using (JsonDocument doc = JsonDocument.Parse(responseString))
				{
					if (doc.RootElement.TryGetProperty("code", out JsonElement codeElement))
					{
						// 输出 "message" 的内容
						if (doc.RootElement.TryGetProperty("message", out JsonElement messageElement))
						{
							throw new Exception(messageElement.GetString());
							//  Console.WriteLine($"返回的值包含 code: {codeElement.GetString()}");
							// Console.WriteLine($"message 的内容是: {messageElement.GetString()}");
						}
					}
				}
				var orderResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ReturnAmount>(responseString);
				await Task.Delay(TimeSpan.FromSeconds(4));
				var status = "";
				if (orderResult.status == "ABNORMAL")
				{
					status = "退款异常";
				}
				else
				if (status == "SUCCESS")
				{
					status = "已退款";
				}
				else
				if (status == "PROCESSING")
				{
					status = "退款处理中";
				}
				else
				{
					status = orderResult.status;
				}

				string payer_refund = (orderResult.amount.payer_refund / 100.0).ToString();

				int update = SQLHelper.ExecuteNonQuery(PayDAL.UpdateOrderRefun(orderResult.out_trade_no, orderResult.transaction_id, orderResult.out_refund_no, orderResult.channel, status, orderResult.user_received_account, payer_refund, id, orderResult.refund_id));

				if (orderResult.status == "PROCESSING")
				{
					status = await GetRefunStatus(orderResult.out_refund_no);
				}
				if (status == "SUCCESS")
				{
					status = "已退款";
				}
				if (status == "PROCESSING")
				{
					status = "退款处理中";
				}
				int update1 = SQLHelper.ExecuteNonQuery(PayDAL.UpdateOrderRefunStatus(status, id, orderResult.out_trade_no));

				errMsg = "OK";
				if (errMsg != "OK")
				{
					throw new Exception(errMsg);
				}
				return DoOKReturn("OK");

			}

			catch (Exception ex)
			{
				return DoErrorReturn(ex.Message);
			}

			//return 
		}
		[HttpGet]
		public async Task<IHttpActionResult> GetRefunOrders(string OpenID)
		{
			string errMsg = "";
			try
			{
				using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.NoRefundOrders(OpenID)))
				{
					foreach (DataRow dr in dt.Rows)
					{
						string Status = await GetRefunStatus(dr["Out_refund_no"].ToString());
						if (Status == "SUCCESS")
						{
							Status = "已退款";
						}
						if (Status == "PROCESSING")
						{
							Status = "退款处理中";
						}
						int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateOrderStatus(OpenID, dr["Out_refund_no"].ToString(), Status));
					}
				}
				errMsg = "OK";
				if (errMsg != "OK")
				{
					throw new Exception(errMsg);
				}
				return DoOKReturn("OK");
			}
			catch (Exception ex)
			{

				return DoErrorReturn(ex.Message);
			}

		}
		public async Task<string> GetRefunStatus(string out_refund_no)
		{

			string MchId = GetMchId();
			string SerialNo = GetSerialNo();
			string appid = GetAppId();
			string method = "GET";
			HttpRequestMessage httpRequest = new HttpRequestMessage();
			httpRequest.Method = HttpMethod.Get;
			httpRequest.Headers.Add("Accept", "*/*");
			httpRequest.Headers.Add("User-Agent", "dotnet/6.0");
			string uri = $"/v3/refund/domestic/refunds/{out_refund_no}";
			var Subtimestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
			int length = 32; // 生成32个十六进制字符
							 //  string randomHexString = GenerateRandomHexString(length);
			string nonce = Helper.Certificates.GenerateRandomHexString(length);
			string message = $"{method}\n{uri}\n{Subtimestamp}\n{nonce}\n\n";

			string signature = Helper.Certificates.Sign(message);
			byte[] s = Encoding.UTF8.GetBytes(message);

			string authrizationValue = $"mchid=\"{MchId}\",nonce_str=\"{nonce}\",timestamp=\"{Subtimestamp}\",serial_no=\"{SerialNo}\",signature=\"{signature}\"";


			httpRequest.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("WECHATPAY2-SHA256-RSA2048", authrizationValue);
			httpRequest.RequestUri = new Uri($"https://api.mch.weixin.qq.com/v3/refund/domestic/refunds/{out_refund_no}");
			var httpClient = new HttpClient();
			HttpResponseMessage response = await httpClient.SendAsync(httpRequest);
			string responseString = await response.Content.ReadAsStringAsync();
			var orderResult = Newtonsoft.Json.JsonConvert.DeserializeObject<ReturnAmount>(responseString);

			return orderResult.status; // 或抛出异常，视具体需求而定
		}

	}
}