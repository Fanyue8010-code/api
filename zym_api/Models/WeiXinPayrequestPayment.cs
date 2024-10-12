using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace zym_api.Models
{
    public class WeiXinPayrequestPayment
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public string timeStamp { get; set; }

        /// <summary>
        /// 随机字符
        /// </summary>
        public string nonceStr { get; set; }

        /// <summary>
        /// 支付的预支付号
        /// </summary>
        public string package { get; set; }

        /// <summary>
        /// 签名类型
        /// </summary>
        public string signType { get; } = "RSA";


        /// <summary>
        /// 小程序支付发起的签名
        /// </summary>
        public string paySign { get; set; }
        public string out_trade_no { get; set; }
    }

}