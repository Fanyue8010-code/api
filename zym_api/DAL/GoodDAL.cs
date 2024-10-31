using System;
using System.Text;
using System.Web.UI;
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
            strBuilder.Append("  ( SELECT [Config2] FROM [dbo].[Config]  WHERE [Config1]='GoodImgPath')+(SELECT  UseValue FROM [dbo].[SysSetting] WHERE [UseFor]='GoodBasicPage') + '/' + CAST(b.ID AS nvarchar(MAX)) + '.jpg' AS Picture ,   ");
            //strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/1bdaf0b0-d242-4e62-b54f-61a1b67fedf5.jpg' AS Picture ,   ");
            // strBuilder.Append("  b.Picture ,   ");
            strBuilder.Append(" CAST(c.Price AS decimal(38,2))  AS  Price,   ");
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
            //strBuilder.Append("  b.Picture ,   ");
            strBuilder.Append(" ( SELECT [Config2] FROM [dbo].[Config]  WHERE [Config1]='GoodImgPath')+(SELECT  UseValue FROM [dbo].[SysSetting] WHERE [UseFor]='GoodBasicPage') + '/' + CAST(b.ID AS nvarchar(MAX)) + '.jpg' AS Picture ,   ");
            //strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/1bdaf0b0-d242-4e62-b54f-61a1b67fedf5.jpg' AS Picture ,   ");
            strBuilder.Append("   CAST(c.Price AS decimal(38,2))  AS  Price,   ");
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
            strBuilder.Append("(SUM(CAST((c.Price* d.GoodQty) AS decimal(38,2))) ) AS  Price,    ");
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
            strBuilder.Append(" AND   d.GoodCheck = 'True'  AND c.Flag='Y' ");
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
            strBuilder.Append("  ( SELECT [Config2] FROM [dbo].[Config]  WHERE [Config1]='GoodImgPath')+(SELECT  UseValue FROM [dbo].[SysSetting] WHERE [UseFor]='GoodBasicPage') + '/' + CAST(b.ID AS nvarchar(MAX)) + '.jpg' AS Picture ,   ");
            //strBuilder.Append("  'http://49.233.191.59/ftp/GoodImg/1bdaf0b0-d242-4e62-b54f-61a1b67fedf5.jpg' AS Picture ,   ");
            // strBuilder.Append("  b.Picture ,   ");
            strBuilder.Append("CAST(c.Price AS decimal(38,2)) AS Price   ,   ");
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
            strBuilder.Append(" IF NOT EXISTS ");
            strBuilder.Append(" (SELECT * FROM   [dbo].[Order]  WHERE  [OpenID]='" + ubi.OpenId + "' AND   [OrderNumber]='" + ubi.OrderNumber + "'  AND  [ShopGoodID]='" + ubi.ShopGoodID + "'   ) ");
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
            //  strBuilder.Append("  ,[Picture]");
            strBuilder.Append("  ,[Price]");
            strBuilder.Append("  ,[GoodQty]");
            if (!string.IsNullOrEmpty(ubi.Status) && ubi.Status == "待发货")
            {
                strBuilder.Append("  ,[CreateTime]");
                strBuilder.Append("  ,[PayTime]");
            }
            //if (!string.IsNullOrEmpty(ubi.Status) && ubi.Status == "待付款")
            //{
            //   // strBuilder.Append("  ,[CreateTime]");
            //}
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
            strBuilder.Append("  ,N'" + ubi.Category + "'");
            strBuilder.Append("  ,N'" + ubi.GoodName + "'");
            // strBuilder.Append(",'" + ubi.Picture + "'");
            strBuilder.Append(",'" + ubi.Price + "'");
            strBuilder.Append(",'" + ubi.GoodQty + "'");
            if (!string.IsNullOrEmpty(ubi.Status) && ubi.Status == "待发货")
            {
                strBuilder.Append(",GETDATE()");
                strBuilder.Append(",GETDATE()");
            }
            //if (!string.IsNullOrEmpty(ubi.Status) && ubi.Status == "待付款")
            //{
            //    strBuilder.Append(",GETDATE()");
            //}
            strBuilder.Append(",GETDATE()");
            strBuilder.Append(",N'" + ubi.Name + "'");
            strBuilder.Append(",'" + ubi.Phone + "'");
            strBuilder.Append(",N'" + ubi.Region + "'");
            strBuilder.Append(",N'" + ubi.Address + "'");
            strBuilder.Append(",N'" + ubi.Status + "'");
            strBuilder.Append(" )");
            return strBuilder.ToString();
        }
        public static string GetOrder(string OpenId, string ID, string menuTapCurrent)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append(" SELECT ");
            strBuilder.Append(" a.[ID]");
            strBuilder.Append(" ,a.[OpenID]");
            strBuilder.Append(" ,a.[CartID]");
            strBuilder.Append(" ,a.[OrderNumber]");
            strBuilder.Append(",a.[GoodBasicID]");
            strBuilder.Append(",a.[CategoryID]");
            strBuilder.Append(" ,a.[ShopGoodID]");
            strBuilder.Append(",a.[Category]");
            strBuilder.Append(",a.[GoodName]");
            //strBuilder.Append(",a.[Picture]");
            strBuilder.Append(" ,   ( SELECT [Config2] FROM [dbo].[Config]  WHERE [Config1]='GoodImgPath')+(SELECT  UseValue FROM [dbo].[SysSetting] WHERE [UseFor]='GoodBasicPage') + '/' + CAST(a.[GoodBasicID] AS nvarchar(MAX)) + '.jpg' AS Picture  ");
            strBuilder.Append(" ,CAST(a.[Price] AS decimal(38,2)) AS Price ");
            strBuilder.Append(" ,a.[GoodQty]");
            strBuilder.Append(" ,CONVERT(varchar, a.[CreateTime], 120) AS [CreateTime]");
            strBuilder.Append(" ,CONVERT(varchar, a.[PayTime], 120) AS [PayTime]");
            strBuilder.Append(" ,CONVERT(varchar, a.[ShipDate], 120) AS [ShipDate]");
            strBuilder.Append("  ,CONVERT(varchar, a.[CompletionTime], 120) AS [CompletionTime]");
            strBuilder.Append("  ,CONVERT(varchar, a.[CancelTime], 120) AS [CancelTime]");
            strBuilder.Append(" ,a.[Name]");
            strBuilder.Append(" ,a.[Phone]");
            strBuilder.Append(" ,a.[Region]");
            strBuilder.Append(" ,a.[Address]");
            strBuilder.Append(" ,a.[Status]");
            strBuilder.Append(" ,b.TotalPrice");
            strBuilder.Append(" ,b.TotalCount ");
            strBuilder.Append(" ,c.TotalPriceRefund ");
            strBuilder.Append(" ,b.Count  ");
            strBuilder.Append(" ,a.[Status] ");
            strBuilder.Append(" ,a.[ApplyPhone]");
            strBuilder.Append(", a.[ApplyReason]");
            strBuilder.Append(", a.[ApplyType]");
            strBuilder.Append(" , a.[ApplyPrice]");
            strBuilder.Append(" , a.[ApplyRemark]");
            strBuilder.Append(" , d.[TotalApplyPrice] ");
            strBuilder.Append("FROM  [dbo].[Order] a ");
            strBuilder.Append("INNER JOIN (SELECT OpenID,[OrderNumber], SUM(CAST(([Price] * GoodQty) AS decimal(38,2))) AS TotalPrice");
            strBuilder.Append("  ,SUM(CAST([GoodQty] AS int)) AS TotalCount,COUNT(*) AS Count    ");
            strBuilder.Append("  FROM [dbo].[Order] WHERE OpenID='" + OpenId + "' AND [Flag] ='Y'  ");
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "1")
            {
                strBuilder.Append(" AND [Status]=N'待付款'  ");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "2")
            {
                strBuilder.Append(" AND ([Status]=N'待发货') ");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "3")
            {
                strBuilder.Append(" AND [Status]=N'待收货'  ");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "4")
            {
                strBuilder.Append(" AND [Status]=N'已完成'  ");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "5")
            {
                strBuilder.Append(" AND ([Status]=N'已取消' OR [Status]=N'已退款' OR [Status]=N'退款处理中'  OR [Status]=N'退款异常')  ");
            }
            strBuilder.Append(" GROUP BY OpenID,[OrderNumber]) b ON ");
            strBuilder.Append("  a.OpenID=b.OpenID AND a.OrderNumber=b.OrderNumber ");
            strBuilder.Append("INNER JOIN (SELECT OpenID,[OrderNumber],  SUM(CAST(([Price] * GoodQty) AS decimal(38,2))) AS TotalPriceRefund");
            strBuilder.Append("  ,SUM(CAST([GoodQty] AS int)) AS TotalCountRefund  ");
            strBuilder.Append("  FROM [dbo].[Order] WHERE OpenID='" + OpenId + "' AND [Flag] ='Y'  ");
            strBuilder.Append(" GROUP BY OpenID,[OrderNumber]) c ON ");
            strBuilder.Append("  a.OpenID=c.OpenID AND a.OrderNumber=c.OrderNumber  ");
            strBuilder.Append("INNER JOIN (SELECT OpenID,[OrderNumber], SUM(CAST(ApplyPrice AS decimal(38,2)))  AS TotalApplyPrice   ");
            strBuilder.Append("  FROM [dbo].[Order] WHERE OpenID='" + OpenId + "' AND [Flag] ='Y'  ");
            strBuilder.Append(" GROUP BY OpenID,[OrderNumber]) d ON ");
            strBuilder.Append("  a.OpenID=d.OpenID AND a.OrderNumber=d.OrderNumber");
            strBuilder.Append(" WHERE a.OpenID='" + OpenId + "'  ");
            strBuilder.Append(" AND  a.[CreateTime] >= DATEADD(YEAR, -1, GETDATE()) ");
            strBuilder.Append(" AND  a.[Flag] ='Y' ");
            if (!string.IsNullOrEmpty(ID))
            {
                strBuilder.Append(" AND a.[OrderNumber]='" + ID + "'");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "1")
            {
                strBuilder.Append(" AND a.[Status]=N'待付款'  ORDER BY    a.[CreateTime]  DESC");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "2")
            {
                strBuilder.Append(" AND a.[Status]=N'待发货' ORDER BY    a.[PayTime]  DESC");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "3")
            {
                strBuilder.Append(" AND a.[Status]=N'待收货'  ORDER BY    a.[ShipDate]  DESC");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "4")
            {
                strBuilder.Append(" AND a.[Status]=N'已完成'  ORDER BY    a.[CompletionTime]  DESC");
            }
            if (!string.IsNullOrEmpty(menuTapCurrent) && menuTapCurrent == "5")
            {
                strBuilder.Append(" AND (a.[Status]=N'已取消' OR a.[Status]=N'已退款' OR [Status]=N'退款处理中'  OR [Status]=N'退款异常')  ORDER BY   a.[CancelTime]  DESC");
            }
            return strBuilder.ToString();
        }
        public static string CancelOrder(string OpenId, string OrderNumber)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  UPDATE [dbo].[Order] SET ");
            strBuilder.Append(" [CancelTime] = GETDATE(),[Status] =N'已取消' ");
            strBuilder.Append("  WHERE [OpenID] = '" + OpenId + "' AND [OrderNumber] =  '" + OrderNumber + "'  ");
            Helper.Log.WriteLog(strBuilder.ToString());
            return strBuilder.ToString();
        }

        public static string GetNeedCancelOrder(string OpenId, string OrderNumber)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  SELECT  Status FROM  [dbo].[Order]  ");
            strBuilder.Append("  WHERE  [OpenID] = '" + OpenId + "' AND [OrderNumber] =  '" + OrderNumber + "'  AND  [Status] =N'待付款' ");
            return strBuilder.ToString();
        }
        public static string NoPayOrder(string OpenId, string OrderNumber)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  UPDATE [dbo].[Order] SET ");
            strBuilder.Append(" [PayTime] = GETDATE(),[Status] =N'待发货' ");
            strBuilder.Append("  WHERE [OpenID] = '" + OpenId + "' AND [OrderNumber] =  '" + OrderNumber + "'  ");
            return strBuilder.ToString();
        }
        public static string RefundOrder(string OpenId, string OrderNumber)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  UPDATE [dbo].[Order] SET ");
            strBuilder.Append(" [CancelTime] = GETDATE(),[Status] =N'已退款' ");
            strBuilder.Append("  WHERE [OpenID] = '" + OpenId + "' AND [OrderNumber] =  '" + OrderNumber + "'  ");
            return strBuilder.ToString();
        }


        public static string NoPayOrders(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  SELECT  b.ID AS ShopGoodID,a.ID AS OrderID   FROM [dbo].[Order] a ");
            strBuilder.Append("  INNER JOIN [dbo].[ShopGood] b ON ");
            strBuilder.Append("  a.ShopGoodID = b.ID ");
            strBuilder.Append("  WHERE b.Flag = 'N' AND a.OpenID = '" + OpenId + "' AND a.Status =N'待付款' ");
            return strBuilder.ToString();
        }

        public static string UpdateOrderFlag(string OpenId, string ShopGoodID, string OrderID)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  UPDATE  [dbo].[Order]  ");
            strBuilder.Append("  SET Flag='N' ");
            strBuilder.Append("  WHERE  OpenID = '" + OpenId + "' ");
            strBuilder.Append("  AND   ShopGoodID='" + ShopGoodID + "'  ");
            strBuilder.Append("  AND   ID='" + OrderID + "'  ");
            strBuilder.Append("  AND   Status =N'待付款'  ");
            return strBuilder.ToString();
        }

        public static string NoRefundOrders(string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  SELECT Out_refund_no   FROM [dbo].[Order] a ");
            strBuilder.Append("  WHERE  a.OpenID = '" + OpenId + "' AND (a.[Status]=N'退款处理中'  OR a.[Status]=N'退款异常'   OR a.[Status]='PROCESSING' OR a.[Status]='ABNORMAL') ");
            return strBuilder.ToString();
        }
        public static string UpdateOrderStatus(string OpenId, string Out_refund_no, string Status)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  UPDATE  [dbo].[Order]  ");
            strBuilder.Append("  SET Status='" + Status + "' ");
            strBuilder.Append("  WHERE  OpenID = '" + OpenId + "' ");
            strBuilder.Append("  AND   Out_refund_no='" + Out_refund_no + "'  ");
            return strBuilder.ToString();
        }

        public static string ApplyRefund(string OpenId, string ID, string Phone, string Reason, string Type, string Price, string Remark)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  UPDATE  [dbo].[Order]  ");
            strBuilder.Append("  SET ApplyPhone=N'" + Phone + "' ");
            strBuilder.Append("  , ApplyReason=N'" + Reason + "' ");
            strBuilder.Append("  , ApplyType=N'" + Type + "' ");
            strBuilder.Append("  , ApplyPrice=N'" + Price + "' ");
            strBuilder.Append("  , ApplyRemark=N'" + Remark + "' ");
            strBuilder.Append("  WHERE  OpenID = '" + OpenId + "' ");
            strBuilder.Append("  AND   ID='" + ID + "'  ");
            return strBuilder.ToString();
        }

        //public static string NoPayOrderCancel(string OpenId, string OrderNumber)
        //{
        //    StringBuilder strBuilder = new StringBuilder();
        //    strBuilder.Append("  UPDATE [dbo].[Order] SET ");
        //    strBuilder.Append(" [PayTime] = GETDATE(),[Status] =N'待付款' ");
        //    strBuilder.Append("  WHERE [OpenID] = '" + OpenId + "' AND [OrderNumber] =  '" + OrderNumber + "' AND ([Status] IS NULL OR [Status]='') ");
        //    return strBuilder.ToString();
        //}
        public static string NoPayOrderStatus(string OpenId, string OrderNumber, string Status)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  UPDATE [dbo].[Order] SET ");
            //  strBuilder.Append("  [Status] =N'"+ Status + "',  [PayTime] = GETDATE() ");
            if (Status == "待发货")
            {
                strBuilder.Append("  [Status] =N'" + Status + "',  [PayTime] = GETDATE() ");
            }
            if (Status == "待付款")
            {
                strBuilder.Append("  [Status] =N'" + Status + "'  ");
            }
            if (Status == "已取消")
            {
                strBuilder.Append("  [Status] =N'" + Status + "',  [CancelTime] = GETDATE() ");
            }
            strBuilder.Append("  WHERE [OpenID] = '" + OpenId + "' AND [OrderNumber] =  '" + OrderNumber + "'  ");
            return strBuilder.ToString();
        }

        public static string GetOrder(string OrderNo, string Status, string Prepare, string Start, string End)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ");
            strBuilder.Append("O.ID, ");
            strBuilder.Append("O.Transaction_id AS OrderNumber, ");
            strBuilder.Append("O.Category, ");
            strBuilder.Append("B.GoodName, ");
            strBuilder.Append("O.Price, ");
            strBuilder.Append("O.GoodQty, ");
            strBuilder.Append("CONVERT(VARCHAR,O.PayTime,120) AS PayTime, ");
            strBuilder.Append("O.Address + '-' + O.Name + '(' + O.Phone + ')' AS Payer, ");
            strBuilder.Append("O.Status, ");
            strBuilder.Append("O.Prepare, ");
            strBuilder.Append("O.OpenID ");
            strBuilder.Append("FROM [Order] O ");
            strBuilder.Append("LEFT JOIN GoodBasic B ");
            strBuilder.Append("ON O.GoodBasicID = B.ID ");
            strBuilder.Append("WHERE 1 = 1  ");
            if(!string.IsNullOrEmpty(Start) && !string.IsNullOrEmpty(End))
            {
                strBuilder.Append("AND CreateTime BETWEEN '"+ Start + "' AND '"+ End + " 23:59:59' ");
            }
            if (!string.IsNullOrEmpty(Status))
            {
                strBuilder.Append("AND STATUS = '" + Status + "' ");
            }
            if (!string.IsNullOrEmpty(OrderNo))
            {
                strBuilder.Append("AND Transaction_id = '" + OrderNo.ToUpper()+"' ");
            }
            if(!string.IsNullOrEmpty(Prepare) &&  Prepare != "全部")
            {
                strBuilder.Append("AND PREPARE = '"+ (Prepare == "已拣货" ? "Y" : "N") +"' ");
            }
            return strBuilder.ToString();
        }

        public static string ChgPrepare(string orderNo, string id)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE [ORDER] SET PREPARE = 'Y' ");
            strBuilder.Append("WHERE Transaction_id = '" + orderNo+ "' AND ID = '" + id+"'");
            return strBuilder.ToString();
        }

        public static string GetFee()
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("select Config3 as fee, Config4 as free  from Config ");
            strBuilder.Append("where Config4 = 'DeliveryFee' or Config4 = 'FreeDeliveryFee' ");
            return strBuilder.ToString();
        }

        public static string ChgFee(string fee, string free)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE Config Set config3 = '" + fee + "' where Config4 = 'DeliveryFee';"); 
            strBuilder.Append("UPDATE Config Set config3 = '" + free + "' where Config4 = 'FreeDeliveryFee';");
            return strBuilder.ToString();
        }

        public static string GetOrderByTransId(string transId)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT OpenID, GoodName, GoodQty FROM [Order] ");
            strBuilder.Append("WHERE Transaction_id = '"+ transId + "' ");
            strBuilder.Append("AND STATUS = '待发货' ");
            return strBuilder.ToString();
        }

        public static string ChgShipStatus(string transId, string status)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE [Order] SET Status = '"+status+ "', ShippingTime = '"+DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"' ");
            strBuilder.Append("WHERE Transaction_id = '" + transId + "' ");
            strBuilder.Append("AND STATUS = '待发货' ");
            return strBuilder.ToString();
        }
    }
}