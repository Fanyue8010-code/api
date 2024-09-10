using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using zym_api.BLL;
using zym_api.DAL;
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
        public string GetAppId()
        {
            DataTable dt = SQLHelper.ExecuteDataTable("select Config2 from [dbo].[Config] where Config1 = 'appid'");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Config2"].ToString();
            }
            return null; // 或抛出异常，视具体需求而定
        }
        public string GetSecret()
        {
            DataTable dt = SQLHelper.ExecuteDataTable("select Config2 from [dbo].[Config] where Config1 = 'secret'");
            if (dt.Rows.Count > 0)
            {
                return dt.Rows[0]["Config2"].ToString();
            }
            return null; // 或抛出异常，视具体需求而定
        }
        string secret = ConfigurationManager.AppSettings["secret"];
        /// <summary>
        /// 根据code获取OpenId
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult Index(string code)
        {


            string appid = GetAppId();
            string secret = GetSecret();
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
        /// <summary>
        /// 根据OpenID查询数据库是否含有用户信息
        /// </summary>
        /// <param name="OpenId"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 没有用户信息则写入信息
        /// </summary>
        /// <param name="OpenId"></param>
        /// <param name="NickName"></param>
        /// <param name="AvatarUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public IHttpActionResult InsertUser(string OpenId, string NickName, string AvatarUrl)
        {
            try
            {
                string errMsg = "";
                string result = LoginBLL.InsertUser(OpenId, NickName, AvatarUrl, out errMsg);
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
        public IHttpActionResult Upload()
        {
            try
            {
                string errMsg = "";
                var files = HttpContext.Current.Request.Files;
                var OpenId = HttpContext.Current.Request.Form["OpenId"];
                var file = HttpContext.Current.Request.Files[0];
                Stream stream = file.InputStream;
                byte[] bs = new byte[(int)stream.Length];
                stream.Read(bs, 0, (int)stream.Length);

                MemoryStream ms = new MemoryStream(bs);
                byte[] b = Helper.ImageHelper.ResizeImage(ms, 1000);
                ms.Dispose();
                ms = null;
                bs = null;
                string AvatarUrl = Convert.ToBase64String(b);
                string result = LoginBLL.UpdateImageUrl(OpenId, b, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                // Handle exceptions
                return DoErrorReturn(ex.Message);
            }
        }

    }
}