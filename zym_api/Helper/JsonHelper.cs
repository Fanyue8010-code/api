﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace zym_api.Helper
{
    public class JsonHelper
    {
        public static dynamic GetJsonDataFromUrl(string sessionUrl)
        {
            // 忽略 SSL 证书验证
            ServicePointManager.ServerCertificateValidationCallback +=
                (sender, certificate, chain, sslPolicyErrors) => true;

            try
            {
                // 创建并配置 HTTP 请求
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(sessionUrl);
                request.Method = "GET";
                request.ContentType = "text/html;charset=UTF-8";

                // 获取响应
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                using (Stream responseStream = response.GetResponseStream())
                using (StreamReader streamReader = new StreamReader(responseStream, Encoding.UTF8))
                {
                    string responseString = streamReader.ReadToEnd();

                    // 解析 JSON 字符串
                    return JsonConvert.DeserializeObject(responseString);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}