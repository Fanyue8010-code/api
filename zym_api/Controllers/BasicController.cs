using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using zym_api.BLL;
using zym_api.Helper;
using zym_api.Models;

namespace zym_api.Controllers
{
    public class BasicController : ApiController
    {
        [HttpGet]
        public HttpResponseMessage GetCategory(string category)
        {
            List<Category> entity = BasicBLL.GetCategory(category);
            string strJson = JsonConvert.SerializeObject(entity);
            HttpResponseMessage msg = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return msg;
        }

        [HttpGet]
        public HttpResponseMessage SaveCategory(string category)
        {
            string strJson = JsonConvert.SerializeObject(BasicBLL.SaveCategory(category));
            HttpResponseMessage msg = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return msg;
        }

        [HttpGet]
        public HttpResponseMessage ChgCategory(string category, string sourceCategory)
        {
            string strJson = JsonConvert.SerializeObject(BasicBLL.ChgCategory(category, sourceCategory));
            HttpResponseMessage msg = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return msg;
        }

        [HttpGet]
        public HttpResponseMessage DelCategory(string category)
        {
            string strJson = JsonConvert.SerializeObject(BasicBLL.DelCategory(category));
            HttpResponseMessage msg = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return msg;
        }

        [HttpGet]
        public HttpResponseMessage SaveGoodBasic(
            string CategoryID,
            string Name,
            string SubPackQty,
            string SubPackBarcode,
            string HasSubPack,
            string SubPackUnit,
            string Unit,
            string Barcode
            )
        {
            GoodBasic entity = new GoodBasic();
            entity.CategoryID = CategoryID;
            entity.Name = Name;
            entity.SubPackQty =SubPackQty;
            entity.SubPackBarcode = SubPackBarcode;
            entity.HasSubPack = HasSubPack == "false" ? "N" : "Y";
            entity.SubPackUnit = SubPackUnit;
            entity.Unit = Unit;
            entity.Barcode = Barcode;
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.SaveGoodBasic(entity));
            }
            catch (Exception ex)
            {
                strJson = ex.Message;
            }
            HttpResponseMessage msg = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return msg;
        }
    }
}