using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zym_api.Models
{
    public class Order
    {

    }

    public class OrderRequestTime
    {
        public long begin_time { get; set; }
        public long end_time { get; set; }
    }
    public class OrderList
    {
        public OrderRequestTime pay_time_range { get; set; } = new OrderRequestTime();
        public int order_state { get; set; }
        public int page_size { get; set; }
    }

    public class OrderResponse
    {
        //交易单号 transaction_id
        public string PaySerialNo { get; set; }
        //内部单号  merchant_trade_no
        public string ShopSerialNo { get; set; }
        public string OpenID { get; set; }
        public string Amount { get; set; }
        public string CreateTime { get; set; }
        public string PayTime { get; set; }
        public string OrderStatus { get; set; }
        public string GoodName { get; set; }
        public string Price { get; set; }
        public string Qty { get; set; }
        public string Buyer { get; set; }
    }

    public class OrderResponseList
    {
        public List<OrderResponse> OrderList { get; set; } = new List<OrderResponse>();
    }

    public class DesOrderList
    {
        public string transaction_id { get; set; }
        public string merchant_trade_no { get; set; }
        public string description { get; set; }
        public string paid_amount { get; set; }
        public string openid { get; set; }
        public string trade_create_time { get; set; }
        public string pay_time { get; set; }
        public string order_state { get; set; }
    }

    public class OrderByTransId
    {
        public string transaction_id { get; set; }
    }

    public class ShipPost
    {
        public ShipOrderKey order_key { get; set; }
        public int delivery_mode { get; set; } = 1;
        public int logistics_type { get; set; } = 2;
        public List<ShipList> shipping_list { get; set; } = new List<ShipList>();
        public string upload_time { get; set; } = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.120+08:00");
        public Payer payer { get; set; }

    }

    public class ShipOrderKey
    {
        public int order_number_type { get; set; } = 2;
        public string transaction_id { get; set; }
    }

    public class ShipList
    {
        public string item_desc { get; set; }
    }

    public class Payer
    {
        public string openid { get; set; }
    }
}