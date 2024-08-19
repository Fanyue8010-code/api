using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using zym_api.BLL;
using zym_api.Helper;
using zym_api.Models;
using static zym_api.Models.LoginModel;




namespace zym_api.Controllers
{
    public class LoginController : ApiController
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
        string appid = ConfigurationManager.AppSettings["appid"];
        string secret = ConfigurationManager.AppSettings["secret"];
        [HttpGet]
        public IHttpActionResult Index(string code)
        {


            //var result = new
            //{
            //    code = 0,
            //    data = jsonData, // 这里根据实际情况填充数据
            //    message = "success" // 这里根据实际情况填充消息
            //};

            try
            {
                string sessionUrl = "https://api.weixin.qq.com/sns/jscode2session?appid=" + appid + "&secret=" + secret + "&js_code=" + code + "& grant_type=authorization_code";
                var jsonData = Helper.JsonHelper.GetJsonDataFromUrl(sessionUrl);

                return DoOKReturn(jsonData);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
            //return Ok(result); // Ok() 方法将对象作为 JSON 返回给客户端
        }
        [HttpGet]
        public IHttpActionResult UserInfo(string OpenId)
        {
            try
            {
                string errMsg = "";
                List<UserInfo> list = LoginBLL.GetUserInfo(OpenId, out errMsg);
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
    }
}