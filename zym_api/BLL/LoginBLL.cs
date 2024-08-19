using System;
using System.Collections.Generic;
using System.Data;
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
                        ubi.AvatarUrl = dr["AvatarUrl"].ToString();
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
    }
}