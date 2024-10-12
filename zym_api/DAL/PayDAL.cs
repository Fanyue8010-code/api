using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Web;

namespace zym_api.DAL
{
    public class PayDAL
    {
        public static string UpdateOrder(string out_trade_no, string transaction_id, string trade_type, string trade_state, string OpenId)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE ");
            strBuilder.Append("[dbo].[Order]  ");
            strBuilder.Append("SET  ");
            strBuilder.Append(" [out_trade_no] ='" + out_trade_no + "' ");
            strBuilder.Append(",[transaction_id]='" + transaction_id + "'");
            strBuilder.Append(" ,[trade_type]='" + trade_type + "'");
            strBuilder.Append(",[trade_state]='" + trade_state + "' ");
            strBuilder.Append("  WHERE [OpenID]='" + OpenId + " ' ");
            strBuilder.Append("  AND ");
            strBuilder.Append("  [OrderNumber]='" + out_trade_no + "'");
            // Helper.Log.WriteLog("insert：" + strBuilder.ToString());
            return strBuilder.ToString();
        }


        public static string UpdateOrderRefun(string out_trade_no, string transaction_id, string out_refund_no, string channel, string status, string user_received_account, string payer_refund, string id, string refund_id)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE ");
            strBuilder.Append("[dbo].[Order]  ");
            strBuilder.Append("SET  ");
            strBuilder.Append(" [CancelTime] = GETDATE() ");
            strBuilder.Append(" ,[Status] =N'" + status + "' ");
            strBuilder.Append(" ,[out_refund_no]='" + out_refund_no + "'");
            strBuilder.Append(",[channel]='" + channel + "' ");
            strBuilder.Append(",[user_received_account]='" + user_received_account + "' ");
            strBuilder.Append(",[payer_refund]='" + payer_refund + "' ");
            strBuilder.Append(",[refund_id]='" + refund_id + "' ");
            strBuilder.Append(" WHERE ");
            strBuilder.Append("  [OrderNumber]='" + out_trade_no + "'  ");
            if (!string.IsNullOrEmpty(id) && id != "undefined")
            {
                strBuilder.Append(" AND ID='" + id + "'");
            }
            return strBuilder.ToString();
        }
        public static string UpdateOrderRefunStatus(string status, string id, string out_trade_no)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE ");
            strBuilder.Append("[dbo].[Order]  ");
            strBuilder.Append("SET  ");
            strBuilder.Append(" [Status] =N'" + status + "' ");
            strBuilder.Append(" WHERE ");
            strBuilder.Append("  [OrderNumber]='" + out_trade_no + "'  ");
            if (!string.IsNullOrEmpty(id) && id != "undefined")
            {
                strBuilder.Append(" AND ID='" + id + "'");
            }
            return strBuilder.ToString();
        }
        //public static string RefundOrdersStatus(string status, string id, string out_trade_no)
        //{
        //    StringBuilder strBuilder = new StringBuilder();
        //    strBuilder.Append("UPDATE ");
        //    strBuilder.Append("[dbo].[Order]  ");
        //    strBuilder.Append("SET  ");
        //    strBuilder.Append(" [Status] =N'" + status + "' ");
        //    strBuilder.Append(" WHERE ");
        //    strBuilder.Append("  [OrderNumber]='" + out_trade_no + "'  ");
        //    if (!string.IsNullOrEmpty(id) && id != "undefined")
        //    {
        //        strBuilder.Append(" AND ID='" + id + "'");
        //    }
        //    return strBuilder.ToString();
        //}

        public static string GetOrdersStatus(string id)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT  Status, OrderNumber ");
            strBuilder.Append(" FROM [dbo].[Order]  ");
            strBuilder.Append(" WHERE ");
            strBuilder.Append("  [ID]='" + id + "'  ");
            return strBuilder.ToString();
        }
        public static string GetOrdersNumber(string out_trade_no)
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("SELECT  Status, OrderNumber  ");
            strBuilder.Append(" FROM [dbo].[Order]  ");
            strBuilder.Append(" WHERE ");
            strBuilder.Append("  [OrderNumber]='" + out_trade_no + "'  AND  [Status] =N'待发货'  ");
            return strBuilder.ToString();
        }

        public static string GetOrderStatus()
        {
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.Append("  SELECT  OpenID,OrderNumber  FROM [dbo].[Order] WHERE (OrderNumber != '' AND  OrderNumber IS NOT NULL) AND (Status IS NULL OR Status = '')  OR    (PayTime IS NULL OR PayTime = '') AND Status='待付款'  AND  OrderNumber<> ''");
            return strBuilder.ToString();
        }
    }
}