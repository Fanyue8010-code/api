using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace zym_api.Helper
{
    public class Helper
    {
        public static string Base64(string val)
        {
            //对输入的密码进行两次转译
            byte[] bytes = System.Text.Encoding.UTF8.GetBytes(val);

            // 对字节数组进行Base64编码
            string base64 = Convert.ToBase64String(bytes);
            bytes = System.Text.Encoding.UTF8.GetBytes(base64);
            base64 = Convert.ToBase64String(bytes);

            bytes = Convert.FromBase64String(base64);
            string data = System.Text.Encoding.UTF8.GetString(bytes);
            return data;
        }

        public static string OrderStatus(string status)
        {
            if (status == "1")
            {
                return "待发货";
            }
            else if (status == "2")
            {
                return "已发货";
            }
            else if (status == "3")
            {
                return "确认收货";
            }
            else if (status == "4")
            {
                return "交易完成";
            }
            else if (status == "5")
            {
                return "已退款";
            }
            return "";
        }
    }
}