using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static zym_api.Models.Refund;

namespace zym_api.Models
{
    /// <summary>
    /// 下单对应的请求DTO
    /// </summary>
    public class WeiXinPayOrderRequestDTO
    {
        /// <summary>
        /// 应用id
        /// </summary>
        public string appid { get; set; }

        /// <summary>
        /// 商户id
        /// </summary>
        public string mchid { get; set; }

        /// <summary>
        /// 商品描述
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// 商户系统内部订单号，只能是数字、大小写字母_-*且在同一个商户号下唯一。
        /// </summary>
        public string out_trade_no { get; set; } = ""; // 默认值为空字符串

        /// <summary>
        /// 订单失效时间，遵循rfc3339标准格式，格式为yyyy-MM-DDTHH:mm:ss+TIMEZONE，yyyy-MM-DD表示年月日，
        /// T出现在字符串中，表示time元素的开头，HH:mm:ss表示时分秒，TIMEZONE表示时区
        /// （+08:00表示东八区时间，领先UTC8小时，即北京时间）。
        /// 例如：2015-05-20T13:29:35+08:00表示，北京时间2015年5月20日 13点29分35秒
        /// </summary>
       // public string time_expire { get; set; }

        /// <summary>
        /// 附加数据，在查询API和支付通知中原样返回，可作为自定义参数使用，实际情况下只有支付完成状态才会返回该字段。
        /// 本系统存放的是订单id
        /// </summary>
      //  public string attach { get; set; }

        /// <summary>
        ///  通知URL必须为直接可访问的URL，不允许携带查询串，要求必须为https地址。
        /// </summary>
        public string notify_url { get; set; }

        /// <summary>
        /// 是否开通发票
        /// </summary>
        //public bool support_fapiao { get; private set; } = false;

        /// <summary>
        /// 订单金额
        /// </summary>
        public WeiXinPayOrderAmout amount { get; set; } = new WeiXinPayOrderAmout();

        /// <summary>
        /// 支付人
        /// </summary>
        public WeiXinPayOrderPayer payer { get; set; } = new WeiXinPayOrderPayer();


        public WeiXinPayOrderDetail detail { get; set; } = new WeiXinPayOrderDetail();
    }


    public class WeiXinPayOrderAmout
    {
        /// <summary>
        ///总金额
        /// </summary>
        public int total { get; set; }

        /// <summary>
        /// CNY：人民币，境内商户号仅支持人民币。
        /// </summary>
        public string currency { get; private set; } = "CNY";
    }
    public class WeiXinPayOrderPayer
    {
        /// <summary>
        /// 小程序对应的用户openid
        /// </summary>
        public string openid { get; set; }
    }

    public class WeiXinPayOrderSettleInfo
    {
        /// <summary>
        /// 是否指定分账
        /// </summary>
        public bool profit_sharing { get; set; }
    }

    public class WeiXinPayOrderDetail 
    {
        /// <summary>
        /// 是否指定分账
        /// </summary>
        public List<DetailModel> goods_detail { get; set; } = new List<DetailModel>(); // 新增属性
    }
    public class DetailModel {
        //【商户侧商品编码】 由半角的大小写字母、数字、中划线、下划线中的一种或几种组成。
        public string merchant_goods_id { get; set; }


        //【商品名称】 商品的实际名称
        public string goods_name { get; set; }

        //【商品数量】 用户购买的数量
        public int quantity { get; set; }

        //【商品单价】 单位为：分。如果商户有优惠，需传输商户优惠后的单价(例如：用户对一笔100元的订单使用了商场发的纸质优惠券100-50，则活动商品的单价应为原单价-50)
        public int unit_price { get; set; }
    }
}