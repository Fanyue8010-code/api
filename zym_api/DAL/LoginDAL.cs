using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;

namespace zym_api.DAL
{
    public class LoginDAL
    {
        public static StringBuilder strBuilder;
        public static string GetUserInfo(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT [ID] ");
            strBuilder.Append(" ,[OpenId] ");
            strBuilder.Append(", [NickName] ");
            strBuilder.Append(",[AvatarUrl] ");
            strBuilder.Append(", [CreateTime] ");
            strBuilder.Append("FROM [dbo].[Users]");
            strBuilder.Append(" WHERE  [OpenId] = '" + OpenId + "' ");
            return strBuilder.ToString();
        }

        public static string InsertUser(string OpenId, string NickName,string AvatarUrl)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO [dbo].[Users]");
            strBuilder.Append(" (");
            strBuilder.Append(" ,[OpenId] ");
            strBuilder.Append(", [NickName] ");
            strBuilder.Append(",[AvatarUrl] ");
            strBuilder.Append(", [CreateTime] ");
            strBuilder.Append(")");
            strBuilder.Append(" VALUES ");
            strBuilder.Append(" (");
            strBuilder.Append("N'" + OpenId + "'");
            strBuilder.Append(",N'" + NickName + "'");
            strBuilder.Append(",N'" + AvatarUrl + "'");
            strBuilder.Append(",GETDATE()");
            strBuilder.Append(")");
            return strBuilder.ToString();
        }
    }
}