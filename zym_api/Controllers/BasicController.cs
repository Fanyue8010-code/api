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
            string Action,
            string ID,
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
            entity.Action = Action;
            if (!string.IsNullOrEmpty(ID))
            {
                entity.GoodID = ID;
            }
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

        public HttpResponseMessage GetGood(string category, string goodName, string barcode, string subBarcode)
        {
            string strJson = "";
            try
            {
                DataTable dt = BasicBLL.GetGood(category, goodName, barcode, subBarcode);
                strJson = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage DelGood(string id)
        {
            string strJson = JsonConvert.SerializeObject(BasicBLL.DelGood(id));
            HttpResponseMessage msg = new HttpResponseMessage { Content = new StringContent(strJson, Encoding.GetEncoding("UTF-8"), "application/json") };
            return msg;
        }

        public HttpResponseMessage GetGoodByBarcode(string barcode)
        {
            string strJson = "";
            try
            {
                DataTable dt = BasicBLL.GetGoodByBarcode(barcode);
                strJson = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage SaveShelf(string shelf, string goodID, string unit)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.SaveShelf(shelf, goodID, unit));
            }
            catch(Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage OffShelf(string shelf, string barcode)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.OffShelf(shelf, barcode));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetShelf(string good, string barcode, string subBarcode, string shelf)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetShelf(good, barcode, subBarcode, shelf));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage ReqGood(string query)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.ReqGood(query));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetSeq()
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetSeq());
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage SaveReq(string id, string goodID, string unit, string qty, string user)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.SaveReq(id, goodID, unit, qty, user));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetReqInfo(string reqNo)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetReqInfo(reqNo));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage DelReq(string reqNo, string goodID, string unit)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.DelReq(reqNo, goodID, unit));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetMenu()
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetMenu());
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage ConfirmReq(string reqNo)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.ConfirmReq(reqNo));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage DelReqNo(string reqNo)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.DelReqNo(reqNo));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetReqNo()
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetReqNo());
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetShelf(string unit, string goodID)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetShelf(unit, goodID));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage IsShelfExist(string shelf)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.IsShelfExist(shelf));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage CheckScanGood(string reqNo, string barcode, string shelf)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.CheckScanGood(reqNo, barcode, shelf));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }
    }
}