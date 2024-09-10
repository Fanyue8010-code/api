using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Linq;
using zym_api.Models;
using static zym_api.Models.GoodModel;

namespace zym_api.DAL
{
    public class GoodDAL
    {
        public static StringBuilder strBuilder;
        public static string GetGoodCategory()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ");
            strBuilder.Append("DISTINCT ");
            strBuilder.Append(" ID ,");
            strBuilder.Append(" Category, ");
            strBuilder.Append(" 'http://49.233.191.59/ftp/CateImg/' + CAST(ID AS nvarchar(MAX)) + '.jpg' AS  Picture ");
            strBuilder.Append(" FROM  ");
            strBuilder.Append(" [dbo].[GoodCategory]  ");
            return strBuilder.ToString();
        }
        public static string GetGoodBasic(string CategoryID, string SearchValue, string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  SELECT  ");
            strBuilder.Append("  d.ID AS CartID,   ");
            strBuilder.Append("  b.ID AS GoodBasicID,   ");
            strBuilder.Append(" b.CategoryID,   ");
            strBuilder.Append(" c.ID AS ShopGoodID, ");
            strBuilder.Append("  a.Category,   ");
            strBuilder.Append("  (b.GoodName + ' | '+ ( CAST(c.Qty AS nvarchar) + c.Unit +'('+ c.Spec+'/'+c.Unit+')'))  AS GoodName,   ");
            strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/' + CAST(b.ID AS nvarchar(MAX)) + '.jpg' AS Picture ,   ");
            //strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/1bdaf0b0-d242-4e62-b54f-61a1b67fedf5.jpg' AS Picture ,   ");
            strBuilder.Append("  c.Price ,   ");
            strBuilder.Append("  d.GoodQty    ");
            strBuilder.Append("  FROM  [dbo].[GoodCategory] a   ");
            strBuilder.Append(" INNER  JOIN [dbo].[GoodBasic] b ON   ");
            strBuilder.Append("  a.ID = b.CategoryID    ");
            strBuilder.Append(" INNER JOIN  [dbo].[ShopGood] c ON ");
            strBuilder.Append("  b.ID = c.GoodID   ");
            strBuilder.Append("  LEFT JOIN ( SELECT  * FROM [dbo].[Cart]  ");
            strBuilder.Append("   WHERE OpenID='" + OpenId + "' ");
            strBuilder.Append("  ) d ON ");
            strBuilder.Append("  b.ID = d.GoodBasicID    ");
            strBuilder.Append("  AND ");
            strBuilder.Append("  a.ID =d.CategoryID ");
            strBuilder.Append("  AND ");
            strBuilder.Append("  c.ID  = d.ShopGoodID ");
            strBuilder.Append("  WHERE  c.Flag='Y' ");
            if (!string.IsNullOrEmpty(CategoryID))
            {
                strBuilder.Append(" AND b.CategoryID='" + CategoryID + "' ");
            }
            if (!string.IsNullOrEmpty(SearchValue))
            {
                strBuilder.Append(" AND (");
                for (int i = 0; i < SearchValue.Length; i++)
                {
                    if (i > 0)
                    {
                        strBuilder.Append(" OR ");
                    }
                    strBuilder.Append("a.Category LIKE '%" + SearchValue[i] + "%' ");
                    strBuilder.Append(" OR b.GoodName LIKE '%" + SearchValue[i] + "%' ");
                }
                strBuilder.Append(") ");
            }
            return strBuilder.ToString();
        }
        public static string InsertCart(string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO [dbo].[Cart] ");
            strBuilder.Append(" ( ");
            strBuilder.Append("[OpenID]");
            strBuilder.Append(" ,[CategoryID]");
            strBuilder.Append(" ,[GoodBasicID]");
            strBuilder.Append(" ,[ShopGoodID]");
            strBuilder.Append(" ,[CategoryCheck]");
            strBuilder.Append(" ,[GoodCheck]");
            strBuilder.Append(" ,[GoodQty]");
            strBuilder.Append(" ,[CreateTime] ");
            strBuilder.Append(")");
            strBuilder.Append(" VALUES ");
            strBuilder.Append(" (");
            strBuilder.Append("N'" + OpenId + "'");
            strBuilder.Append(",N'" + CategoryID + "'");
            strBuilder.Append(",N'" + GoodBasicID + "'");
            strBuilder.Append(",N'" + ShopGoodID + "'");
            strBuilder.Append(",N'" + CategoryCheck + "'");
            strBuilder.Append(",N'" + GoodCheck + "'");
            strBuilder.Append(",N'" + GoodQty + "'");
            strBuilder.Append(",GETDATE()");
            strBuilder.Append(")");
            return strBuilder.ToString();
        }
        public static string GetCart(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  SELECT  ");
            strBuilder.Append("d.CategoryCheck ,");
            strBuilder.Append("d.GoodCheck ,");
            strBuilder.Append("  d.ID AS CartID,   ");
            strBuilder.Append("  b.ID AS GoodBasicID,   ");
            strBuilder.Append(" b.CategoryID,   ");
            strBuilder.Append(" c.ID AS ShopGoodID, ");
            strBuilder.Append("  a.Category,   ");
            strBuilder.Append("  (b.GoodName + ' | '+ ( CAST(c.Qty AS nvarchar) + c.Unit +'('+ c.Spec+'/'+c.Unit+')'))  AS GoodName,   ");
            strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/' + CAST(b.ID AS nvarchar(MAX)) + '.jpg' AS Picture ,   ");
            //strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/1bdaf0b0-d242-4e62-b54f-61a1b67fedf5.jpg' AS Picture ,   ");
            strBuilder.Append("  c.Price ,   ");
            strBuilder.Append("  d.GoodQty    ");
            strBuilder.Append("  FROM  [dbo].[GoodCategory] a   ");
            strBuilder.Append(" INNER  JOIN [dbo].[GoodBasic] b ON   ");
            strBuilder.Append("  a.ID = b.CategoryID    ");
            strBuilder.Append(" INNER JOIN  [dbo].[ShopGood] c ON ");
            strBuilder.Append("  b.ID = c.GoodID   ");
            strBuilder.Append("  INNER JOIN ( SELECT  * FROM [dbo].[Cart]  ");
            strBuilder.Append("   WHERE OpenID='" + OpenId + "' ");
            strBuilder.Append("  ) d ON ");
            strBuilder.Append("  b.ID = d.GoodBasicID    ");
            strBuilder.Append("  AND ");
            strBuilder.Append("  a.ID =d.CategoryID ");
            strBuilder.Append("  AND ");
            strBuilder.Append("  c.ID  = d.ShopGoodID ");
            strBuilder.Append("  WHERE  c.Flag='Y'  ");
            return strBuilder.ToString();
        }
        public static string GetCartGoodSum(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT   ");
            strBuilder.Append("(SUM(c.Price) * SUM(d.GoodQty)) AS  Price,    ");
            strBuilder.Append("SUM(d.GoodQty) AS GoodQty ");
            strBuilder.Append("FROM  [dbo].[GoodCategory] a    ");
            strBuilder.Append("INNER  JOIN [dbo].[GoodBasic] b ON    ");
            strBuilder.Append(" a.ID = b.CategoryID    ");
            strBuilder.Append("INNER JOIN  [dbo].[ShopGood] c ON  ");
            strBuilder.Append(" b.ID = c.GoodID    ");
            strBuilder.Append("INNER JOIN ( SELECT  * FROM [dbo].[Cart]   ");
            strBuilder.Append(" WHERE OpenID='" + OpenId + "'  ");
            strBuilder.Append(") d ON  ");
            strBuilder.Append("b.ID = d.GoodBasicID     ");
            strBuilder.Append("AND  ");
            strBuilder.Append("a.ID =d.CategoryID  ");
            strBuilder.Append("AND  ");
            strBuilder.Append(" c.ID  = d.ShopGoodID  ");
            strBuilder.Append(" AND   d.GoodCheck = 'True'  AND c.Flag='Y'");
            return strBuilder.ToString();
        }
        public static string MinusAmount(string OpenId, string CratId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" SELECT (GoodQty-1)  AS GoodQty ");
            strBuilder.Append("FROM [dbo].[Cart] ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append("AND ");
            strBuilder.Append(" [ID]='" + CratId + "' ");
            return strBuilder.ToString();
        }
        public static string DeleteCart(string OpenId, string CratId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" DELETE ");
            strBuilder.Append("FROM [dbo].[Cart] ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append("AND ");
            strBuilder.Append(" [ID]='" + CratId + "' ");
            return strBuilder.ToString();
        }

        public static string UpdateCart(string OpenId, string CratId, int Qty)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" UPDATE  [dbo].[Cart] SET ");
            strBuilder.Append("GoodQty = '" + Qty + "', ");
            strBuilder.Append("UpdateTime=GETDATE() ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append("AND ");
            strBuilder.Append(" [ID]='" + CratId + "' ");
            return strBuilder.ToString();
        }

        public static string PlusAmount(string OpenId, string CratId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" SELECT (GoodQty+1)  AS GoodQty ");
            strBuilder.Append("FROM [dbo].[Cart] ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append("AND ");
            strBuilder.Append(" [ID]='" + CratId + "' ");
            return strBuilder.ToString();
        }
        public static string UpdateCategoryCheck(string OpenId, string CategoryID, string CategoryCheck)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" UPDATE  [dbo].[Cart] SET ");
            strBuilder.Append("CategoryCheck = '" + CategoryCheck + "',");
            strBuilder.Append("GoodCheck='" + CategoryCheck + "', ");
            strBuilder.Append("UpdateTime=GETDATE() ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append("AND ");
            strBuilder.Append(" CategoryID='" + CategoryID + "' ");
            return strBuilder.ToString();
        }
        public static string UpdateGoodCheck(string OpenId, string CartID, string GoodCheck)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" UPDATE  [dbo].[Cart] SET ");
            strBuilder.Append("CategoryCheck = '" + GoodCheck + "',");
            strBuilder.Append("GoodCheck='" + GoodCheck + "', ");
            strBuilder.Append("UpdateTime=GETDATE() ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append("AND ");
            strBuilder.Append(" ID='" + CartID + "' ");
            return strBuilder.ToString();
        }
        public static string UpdateCheckedAll(string OpenId, string checkedAll)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" UPDATE  [dbo].[Cart] SET ");
            strBuilder.Append("CategoryCheck = '" + checkedAll + "',");
            strBuilder.Append("GoodCheck='" + checkedAll + "', ");
            strBuilder.Append("UpdateTime=GETDATE() ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            return strBuilder.ToString();
        }
        public static string DelCart(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" DELETE FROM  [dbo].[Cart] ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append(" AND GoodCheck='True' ");
            return strBuilder.ToString();
        }

        public static string GoodToCart(string CartID, string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" IF NOT EXISTS ");
            strBuilder.Append(" (SELECT * FROM    [dbo].[Cart] WHERE  [OpenID]='" + OpenId + "' AND   [ShopGoodID]='" + ShopGoodID + "'      ) ");
            strBuilder.Append("INSERT INTO [dbo].[Cart] ");
            strBuilder.Append(" ( ");
            strBuilder.Append("[OpenID]");
            strBuilder.Append(" ,[CategoryID]");
            strBuilder.Append(" ,[GoodBasicID]");
            strBuilder.Append(" ,[ShopGoodID]");
            strBuilder.Append(" ,[CategoryCheck]");
            strBuilder.Append(" ,[GoodCheck]");
            strBuilder.Append(" ,[GoodQty]");
            strBuilder.Append(" ,[CreateTime] ");
            strBuilder.Append(")");
            strBuilder.Append(" VALUES ");
            strBuilder.Append(" (");
            strBuilder.Append("N'" + OpenId + "'");
            strBuilder.Append(",N'" + CategoryID + "'");
            strBuilder.Append(",N'" + GoodBasicID + "'");
            strBuilder.Append(",N'" + ShopGoodID + "'");
            strBuilder.Append(",N'" + CategoryCheck + "'");
            strBuilder.Append(",N'" + GoodCheck + "'");
            strBuilder.Append(",N'" + GoodQty + "'");
            strBuilder.Append(",GETDATE()");
            strBuilder.Append(") ");
            strBuilder.Append("ELSE ");
            strBuilder.Append(" UPDATE  [dbo].[Cart] SET ");
            strBuilder.Append("GoodQty = (SELECT (GoodQty+1) AS GoodQty  FROM [dbo].[Cart] WHERE  [OpenID]='" + OpenId + "' AND   [ShopGoodID]='" + ShopGoodID + "' ), ");
            strBuilder.Append("UpdateTime=GETDATE() ");
            strBuilder.Append("WHERE ");
            strBuilder.Append("[OpenID]='" + OpenId + "' ");
            strBuilder.Append("AND ");
            strBuilder.Append("  [ShopGoodID]='" + ShopGoodID + "'  ");
            return strBuilder.ToString();
        }
        public static string GetCartGoodToPay(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  SELECT  ");
            strBuilder.Append("d.CategoryCheck ,");
            strBuilder.Append("d.GoodCheck ,");
            strBuilder.Append("  d.ID AS CartID,   ");
            strBuilder.Append("  b.ID AS GoodBasicID,   ");
            strBuilder.Append(" b.CategoryID,   ");
            strBuilder.Append(" c.ID AS ShopGoodID, ");
            strBuilder.Append("  a.Category,   ");
            strBuilder.Append("  (b.GoodName + ' | '+ ( CAST(c.Qty AS nvarchar) + c.Unit +'('+ c.Spec+'/'+c.Unit+')'))  AS GoodName,   ");
            strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/' + CAST(b.ID AS nvarchar(MAX)) + '.jpg' AS Picture ,   ");
            //strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/1bdaf0b0-d242-4e62-b54f-61a1b67fedf5.jpg' AS Picture ,   ");
            strBuilder.Append("  c.Price ,   ");
            strBuilder.Append("  d.GoodQty    ");
            strBuilder.Append("  FROM  [dbo].[GoodCategory] a   ");
            strBuilder.Append(" INNER  JOIN [dbo].[GoodBasic] b ON   ");
            strBuilder.Append("  a.ID = b.CategoryID    ");
            strBuilder.Append(" INNER JOIN  [dbo].[ShopGood] c ON ");
            strBuilder.Append("  b.ID = c.GoodID   ");
            strBuilder.Append("  INNER JOIN ( SELECT  * FROM [dbo].[Cart]  ");
            strBuilder.Append("   WHERE OpenID='" + OpenId + "' ");
            strBuilder.Append("  ) d ON ");
            strBuilder.Append("  b.ID = d.GoodBasicID    ");
            strBuilder.Append("  AND ");
            strBuilder.Append("  a.ID =d.CategoryID ");
            strBuilder.Append("  AND ");
            strBuilder.Append("  c.ID  = d.ShopGoodID ");
            strBuilder.Append("  WHERE  c.Flag='Y' AND d.GoodCheck = 'True'  ");
            return strBuilder.ToString();
        }

        public static string InsertOrder(GoodsPay ubi)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  INSERT INTO [dbo].[Order] ");
            strBuilder.Append(" (");
            strBuilder.Append(" [OpenID]");
            if (!string.IsNullOrEmpty(ubi.CartID))
            {
                strBuilder.Append("  ,[CartID]");
            }
            strBuilder.Append(" ,[OrderNumber]");
            strBuilder.Append(" ,[GoodBasicID]");
            strBuilder.Append(" ,[CategoryID]");
            strBuilder.Append(" ,[ShopGoodID]");
            strBuilder.Append("  ,[Category]");
            strBuilder.Append("  ,[GoodName]");
            strBuilder.Append("  ,[Picture]");
            strBuilder.Append("  ,[Price]");
            strBuilder.Append("  ,[GoodQty]");
            strBuilder.Append("  ,[CreateTime]");
            strBuilder.Append("  ,[Name]");
            strBuilder.Append("  ,[Phone]");
            strBuilder.Append("  ,[Region]");
            strBuilder.Append("  ,[Address]");
            strBuilder.Append(" ,[Status] ");
            strBuilder.Append(" )");
            strBuilder.Append(" VALUES ");
            strBuilder.Append(" (");
            strBuilder.Append("'" + ubi.OpenId + "'");
            if (!string.IsNullOrEmpty(ubi.CartID))
            {
                strBuilder.Append(",'" + ubi.CartID + "'");
            }
            strBuilder.Append(",'" + ubi.OrderNumber + "'");
            strBuilder.Append(",'" + ubi.GoodBasicID + "'");
            strBuilder.Append(",'" + ubi.CategoryID + "'");
            strBuilder.Append(" ,'" + ubi.ShopGoodID + "'");
            strBuilder.Append("  ,'" + ubi.Category + "'");
            strBuilder.Append("  ,'" + ubi.GoodName + "'");
            strBuilder.Append(",'" + ubi.Picture + "'");
            strBuilder.Append(",'" + ubi.Price + "'");
            strBuilder.Append(",'" + ubi.GoodQty + "'");
            strBuilder.Append(",GETDATE()");
            strBuilder.Append(",'" + ubi.Name + "'");
            strBuilder.Append(",'" + ubi.Phone + "'");
            strBuilder.Append(",'" + ubi.Region + "'");
            strBuilder.Append(",'" + ubi.Address + "'");
            strBuilder.Append(",'" + ubi.Status + "'");
            strBuilder.Append(" )");
            return strBuilder.ToString();
        }
        public static string GetOrder(string OpenId, string ID, string menuTapCurrent)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" SELECT ");
            strBuilder.Append(" a.[OpenID]");
            strBuilder.Append(" ,a.[CartID]");
            strBuilder.Append(" ,a.[OrderNumber]");
            strBuilder.Append(",a.[GoodBasicID]");
            strBuilder.Append(",a.[CategoryID]");
            strBuilder.Append(" ,a.[ShopGoodID]");
            strBuilder.Append(",a.[Category]");
            strBuilder.Append(",a.[GoodName]");
            strBuilder.Append(",a.[Picture]");
            strBuilder.Append(" ,a.[Price]");
            strBuilder.Append(" ,a.[GoodQty]");
            strBuilder.Append(" ,CONVERT(varchar, a.[CreateTime], 120) AS [CreateTime]");
            strBuilder.Append(" ,CONVERT(varchar, a.[PayTime], 120) AS [PayTime]");
            strBuilder.Append(" ,CONVERT(varchar, a.[ShipDate], 120) AS [ShipDate]");
            strBuilder.Append("  ,CONVERT(varchar, a.[CompletionTime], 120) AS [CompletionTime]");
            strBuilder.Append(" ,a.[Name]");
            strBuilder.Append(" ,a.[Phone]");
            strBuilder.Append(" ,a.[Region]");
            strBuilder.Append(" ,a.[Address]");
            strBuilder.Append(" ,a.[Status]");
            strBuilder.Append(" ,b.TotalPrice");
            strBuilder.Append(" ,b.totalCount ");
            strBuilder.Append(" ,a.[Status] ");
            strBuilder.Append("FROM  [dbo].[Order] a ");
            strBuilder.Append("INNER JOIN (SELECT OpenID,[OrderNumber], SUM([Price]) AS TotalPrice");
            strBuilder.Append("  ,SUM(CAST([GoodQty] AS int)) AS TotalCount  ");
            strBuilder.Append("  FROM [dbo].[Order] WHERE OpenID='" + OpenId + "' GROUP BY OpenID,[OrderNumber]) b ON");
            strBuilder.Append("  a.OpenID=b.OpenID AND a.OrderNumber=b.OrderNumber");
            strBuilder.Append(" WHERE a.OpenID='" + OpenId + "' ");
            if (!string.IsNullOrEmpty(ID))
            {
                strBuilder.Append(" AND a.[OrderNumber]='" + ID + "'");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "1")
            {
                strBuilder.Append(" AND a.[Status]=N'待付款'");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "2")
            {
                strBuilder.Append(" AND a.[Status]=N'待发货'");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "3")
            {
                strBuilder.Append(" AND a.[Status]=N'待收货'");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "4")
            {
                strBuilder.Append(" AND a.[Status]=N'已完成'");
            }
            return strBuilder.ToString();
        }
    }
}