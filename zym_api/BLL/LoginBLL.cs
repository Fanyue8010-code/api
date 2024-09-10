using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using zym_api.DAL;
using zym_api.Helper;
using zym_api.Models;
using static zym_api.Models.LoginModel;

namespace zym_api.BLL
{
    public class LoginBLL
    {
        public static List<UserInfo> GetUserInfo(string openId, out string errMsg)
        {
            errMsg = "OK";
            List<UserInfo> list = new List<UserInfo>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(LoginDAL.GetUserInfo(openId)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        UserInfo ubi = new UserInfo();
                        ubi.NickName = dr["NickName"].ToString();
                        byte[] b = (byte[])dr["AvatarUrl"];
                        ubi.AvatarUrl =Convert.ToBase64String(b);
                        list.Add(ubi);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return list;
            }
        }
        public static string InsertUser(string OpenId, string NickName, string AvatarUrl, out string errMsg)
        {
            errMsg = "OK";
            try
            { // 下载并转换图片
              //   string base64String = GetAvatarUrlHelper.DownloadAndConvertToBase64(AvatarUrl);
                int dt = SQLHelper.ExecuteNonQuery(LoginDAL.InsertUser(OpenId, NickName, AvatarUrl));
               return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string UpdateImageUrl(string OpenId, byte[] image,out string errMsg)
        {  errMsg = "OK";
            try {
               int dt = SQLHelper.ExecuteNonQueryUrl(LoginDAL.UpdateImageUrl(OpenId, image),OpenId, image);
                return "OK";
            } 
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
    }
}