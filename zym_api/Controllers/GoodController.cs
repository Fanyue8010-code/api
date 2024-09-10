using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using zym_api.BLL;
using zym_api.Models;
using static zym_api.Models.AddressModel;
using static zym_api.Models.GoodModel;
using System.Configuration;

namespace zym_api.Controllers
{
    public class GoodController : ApiController
    {
        private IHttpActionResult DoOKReturn(object returnObj)
        {
            return DoReturn("0", returnObj, null);
        }
        private IHttpActionResult DoErrorReturn(string errorMsg)
        {
            return DoReturn(null, null, errorMsg);
        }
        private IHttpActionResult DoReturn(string code, object returnObj, string message)
        {
            QueryReturnModel qre = new QueryReturnModel();
            qre.code = code;
            qre.data = returnObj;
            qre.message = message;
            return Ok(qre);
        }
        [HttpGet]
        public IHttpActionResult GetGoodCategory()
        {
            try
            {
                string errMsg = "";
                List<GoodCategory> list = GoodBLL.GetGoodCategory(out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(list);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }

        [HttpGet]
        public IHttpActionResult GetGoodBasic(string CategoryID, string SearchValue, string OpenId)
        {
            try
            {
                string errMsg = "";
                List<GoodCategory> list = GoodBLL.GetGoodBasic(CategoryID, SearchValue, out errMsg, OpenId);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(list);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult InsertCart(string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.InsertCart(OpenId, CategoryID, GoodBasicID, ShopGoodID, CategoryCheck, GoodCheck, GoodQty, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCart(string OpenId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GetCart(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCartGoodSum(string OpenId)
        {
            try
            {
                string errMsg = "";
                List<CartGoodSum> list = GoodBLL.GetCartGoodSum(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(list);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult MinusAmount(string OpenId, string CratId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.MinusAmount(OpenId, CratId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult PlusAmount(string OpenId, string CratId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.PlusAmount(OpenId, CratId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult UpdateCategoryCheck(string OpenId, string CategoryID, string CategoryCheck)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.UpdateCategoryCheck(OpenId, CategoryID, CategoryCheck, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult UpdateGoodCheck(string OpenId, string CartID, string GoodCheck)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.UpdateGoodCheck(OpenId, CartID, GoodCheck, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult UpdateCheckedAll(string OpenId, string checkedAll)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.UpdateCheckedAll(OpenId, checkedAll, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult DelCart(string OpenId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.DelCart(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GoodToCart(string CartID, string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GoodToCart(CartID, OpenId, CategoryID, GoodBasicID, ShopGoodID, CategoryCheck, GoodCheck, GoodQty, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetCartGoodToPay(string OpenId)
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GetCartGoodToPay(OpenId, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpPost]
        public IHttpActionResult InsertPayOrder(JObject jsonObj)
        {
            try
            {
                var jsonStr = JsonConvert.SerializeObject(jsonObj);
                var jsonParams = JsonConvert.DeserializeObject<dynamic>(jsonStr);
                string Data = jsonParams.Orders.ToString().Replace("\n", "").Replace("\r", "");
                if (Data.StartsWith("{"))
                {
                    Data = "[" + Data + "]";
                }
                
                string OpenId = jsonParams.OpenId;
                string Name = jsonParams.Name;
                string Phone = jsonParams.Phone;
                string Region = jsonParams.Region;
                string Address = jsonParams.Address;
                string errMsg = "";
                string result = "";
                if (jsonParams.Type == "CartData")
                { 
                List<GoodsPayList> goodsPays = JsonConvert.DeserializeObject<List<GoodsPayList>>(Data); 
                    result = GoodBLL.InsertPayOrder(goodsPays, OpenId, Name, Phone, Region, Address, out errMsg);
                } else
                {
                  List<GoodsPay> goodsPays = JsonConvert.DeserializeObject<List<GoodsPay>>(Data); 
                     result = GoodBLL.InsertPayOrder(goodsPays, OpenId, Name, Phone, Region, Address, out errMsg);
                }
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        }
        [HttpGet]
        public IHttpActionResult GetOrder(string OpenId,string ID="",string menuTapCurrent="")
        {
            try
            {
                string errMsg = "";
                string result = GoodBLL.GetOrder(OpenId,ID,menuTapCurrent, out errMsg);
                if (errMsg != "OK")
                {
                    throw new Exception(errMsg);
                }
                return DoOKReturn(result);
            }
            catch (Exception ex)
            {
                return DoErrorReturn(ex.Message);
            }
        } 
    }
}