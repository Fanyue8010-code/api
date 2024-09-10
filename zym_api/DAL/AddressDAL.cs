using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace zym_api.DAL
{
    public class AddressDAL
    {
        public static string GetAddressInfo(string OpenId, string Name, string Phone, string Region, string Address, string Status, string Id)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT * FROM [dbo].[Address] WHERE [Name] = N'" + Name + "' AND [Phone] = '" + Phone + "' AND [Region] = N'" + Region + "' AND [Address] = N'" + Address + "' AND [OpenId] = '" + OpenId + "' ND ID='" + Id + "'");
            return strBuilder.ToString();
        }
        public static string GetAddressInfoStatus(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT * FROM [dbo].[Address] WHERE  [OpenId] = '" + OpenId + "' AND  [Status] = 'True'");
            return strBuilder.ToString();
        }
        public static string UpdateStatus(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE  [dbo].[Address]  SET   [Status] = 'False' WHERE  [OpenId] = '" + OpenId + "' AND  [Status] = 'True'");
            return strBuilder.ToString();
        }
        public static string InsertAddressInfo(string OpenId, string Name, string Phone, string Region, string Address, string Status)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("IF NOT EXISTS ");
            strBuilder.Append("(SELECT * FROM [dbo].[Address] WHERE [Name] = N'" + Name + "' AND [Phone] = '" + Phone + "' AND [Region] = N'" + Region + "' AND [Address] = N'" + Address + "' AND [OpenId] = '" + OpenId + "') ");
            strBuilder.Append("INSERT INTO  ");
            strBuilder.Append("[dbo].[Address] ");
            strBuilder.Append(" ( ");
            strBuilder.Append("   [OpenId] ");
            strBuilder.Append("  , [Name] ");
            strBuilder.Append("  , [Phone] ");
            strBuilder.Append("  , [Region] ");
            strBuilder.Append("  , [Address] ");
            strBuilder.Append("  , [Status] ");
            strBuilder.Append(" , [CreateTime] ");
            strBuilder.Append(" ) ");
            strBuilder.Append(" VALUES ");
            strBuilder.Append("( ");
            strBuilder.Append("N'" + OpenId + "', ");
            strBuilder.Append("N'" + Name + "', ");
            strBuilder.Append(" '" + Phone + "', ");
            strBuilder.Append(" N'" + Region + "', ");
            strBuilder.Append(" N'" + Address + "', ");
            strBuilder.Append("N'" + Status + "', ");
            strBuilder.Append(" GETDATE() ");
            strBuilder.Append(" ) ");
            return strBuilder.ToString();
        }
        public static string GetAddressInfo(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" SELECT [ID] ");
            strBuilder.Append(" , [Name] ");
            strBuilder.Append(" , [Phone] ");
            strBuilder.Append(" , [Region] ");
            strBuilder.Append(" , [Address] ");
            strBuilder.Append(" , [Status] ");
            strBuilder.Append("     FROM[dbo].[Address] WHERE [OpenId] = '" + OpenId + "'  ORDER BY Status DESC ");
            return strBuilder.ToString();
        }
        public static string UpdateAddressInfo(string OpenId, string Name, string Phone, string Region, string Address, string Status, string Id)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE   ");
            strBuilder.Append("[dbo].[Address] ");
            strBuilder.Append("SET   ");
            strBuilder.Append("   [Name]=N'" + Name + "' ");
            strBuilder.Append("  , [Phone] ='" + Phone + "'");
            strBuilder.Append("  , [Region] =N'" + Region + "' ");
            strBuilder.Append("  , [Address]=N'" + Address + "'");
            strBuilder.Append("  , [Status]=N'" + Status + "' ");
            strBuilder.Append(" , [CreateTime] =GETDATE() ");
            strBuilder.Append(" WHERE ID='" + Id + "' AND [OpenId] = '" + OpenId + "'");
            return strBuilder.ToString();
        }
        public static string DeleteAdderss(string OpenId, string Id)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("DELETE   FROM ");
            strBuilder.Append("[dbo].[Address] ");
            strBuilder.Append(" WHERE ID='" + Id + "' AND [OpenId] = '" + OpenId + "'");
            return strBuilder.ToString();
        }

        public static string GetAddressTrue(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" SELECT [ID] ");
            strBuilder.Append(" , [Name] ");
            strBuilder.Append(" , [Phone] ");
            strBuilder.Append(" , [Region] ");
            strBuilder.Append(" , [Address] ");
            strBuilder.Append(" , [Status] ");
            strBuilder.Append("     FROM[dbo].[Address] WHERE [OpenId] = '" + OpenId + "'  AND  Status = 'True'");
            return strBuilder.ToString();
        }
    }
}