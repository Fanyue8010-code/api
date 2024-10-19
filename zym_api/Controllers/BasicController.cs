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

        [HttpPost]
        public HttpResponseMessage SaveGoodBasic(JObject json
            //string Action,
            //string ID,
            //string CategoryID,
            //string Name,
            //string SubPackQty,
            //string SubPackBarcode,
            //string HasSubPack,
            //string SubPackUnit,
            //string Unit,
            //string Barcode,
            //string Picture
            )
        {
            var jsonStr = JsonConvert.SerializeObject(json);
            var jsonPara = JsonConvert.DeserializeObject<dynamic>(jsonStr);

            GoodBasic entity = new GoodBasic();
            entity.Action = jsonPara.Action.ToString();
            if (!string.IsNullOrEmpty(jsonPara.ID.ToString()))
            {
                entity.GoodID = jsonPara.ID.ToString();
            }
            entity.ID = Guid.NewGuid();
            entity.CategoryID = jsonPara.CategoryID.ToString();
            entity.Name = jsonPara.Name.ToString();
            entity.SubPackQty = jsonPara.SubPackQty.ToString();
            entity.SubPackBarcode = jsonPara.SubPackBarcode.ToString();
            entity.HasSubPack = jsonPara.HasSubPack.ToString().ToUpper() == "FALSE" ? "N" : "Y";
            entity.SubPackUnit = jsonPara.SubPackUnit.ToString();
            entity.Unit = jsonPara.Unit.ToString();
            entity.Barcode = jsonPara.Barcode.ToString();
            entity.Picture = jsonPara.Picture.ToString();
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.SaveGoodBasic(entity));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
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

        public HttpResponseMessage GetGoodByBarcode(string shelf, string barcode)
        {
            string strJson = "";
            try
            {
                DataTable dt = BasicBLL.GetGoodByBarcode(shelf, barcode);
                strJson = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage SaveShelf(string shelf, string barcode, string unit)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.SaveShelf(shelf, barcode, unit));
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
        public HttpResponseMessage OffShelfOutPack(string pack, string shelf, string barcode)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.OffShelfOutPack(pack, shelf, barcode));
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
        public HttpResponseMessage ReqGood(string reqID, string query)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.ReqGood(reqID, query));
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

        [HttpGet]
        public HttpResponseMessage CheckReqScan(string reqNo, string barcode, string shelf)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.CheckReqScan(reqNo, barcode, shelf));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage OpenPic(string id)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.OpenPic(id));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpPost]
        public HttpResponseMessage UploadFTP()
        {
            string strJson = "";
            try
            {
                string post = HttpContext.Current.Request.HttpMethod;
                string type = HttpContext.Current.Request.ContentType;
                string strFtpPath = HttpContext.Current.Request.Form["url"];
                string strFtpGuid = HttpContext.Current.Request.Form["guid"];
                var file = HttpContext.Current.Request.Files[0];
                bool isCheckFtp = FTPHelper.CheckFtp();
                string strDelete = HttpContext.Current.Request.Form["del"];
                string[] arrPath = strFtpPath.Split('/');
                if (arrPath.Length > 0)
                {
                    string strPart = string.Empty;
                    foreach (string strPath in arrPath)
                    {
                        strPart += strPath + "/";
                        if (strPath.Split('-').Length == 5)
                        {
                            string[] list = FTPHelper.GetFileList(FTPHelper.strFtpPath + strPart);
                            if (strDelete == "true" && list.Length > 0)
                            {
                                FTPHelper.Delete(FTPHelper.strFtpPath + strPart + list[0]);
                            }
                        }
                        if (!FTPHelper.DirectoryExists(FTPHelper.strFtpPath + strPart))
                        {
                            FTPHelper.MakeDirectory(FTPHelper.strFtpPath + strPart);
                        }
                    }
                }

                if (isCheckFtp)
                {
                    FTPHelper.Upload(file, strFtpPath, strFtpGuid);
                }
                strJson = JsonConvert.SerializeObject("");
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }


        [HttpGet]
        public HttpResponseMessage GetGoodByID(string unit, string goodID, string qty)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetGoodByID(goodID, unit, qty));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage SaveShopGood(string goodID, string quantity, string unit, string spec, string exp, string expUnit, string save, string price, string qy)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.SaveShopGood(goodID, quantity, unit, spec, exp, expUnit, save, price, qy));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetGoods()
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetGoods());
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetQY()
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetQY());
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage SaveQY(string id, string value)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.SaveQY(id, value));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetShopGoodByID(string id)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetShopGoodByID(id));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage UpdShopGoodByID(string id, string qty, string unit, string spec, string exp, string expUnit, string save, string price, string qy)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.UpdShopGoodByID(id, qty, unit, spec, exp, expUnit, save, price, qy));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage ChgFlag(string id, string flag)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.ChgFlag(id, flag));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage CheckPsd(string id, string psd)
        {
            string strJson = "";
            try
            {
                string strPsd = Helper.Helper.Base64(psd);

                //从数据库取密码
                DataTable dt = BasicBLL.GetSysUser(id, psd);
                if(dt.Rows.Count > 0)
                {
                    if(strPsd != dt.Rows[0]["PSD"].ToString())
                    {
                        throw new Exception("密码输入错误");
                    }
                }
                strJson = JsonConvert.SerializeObject(dt);
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage ChgPsd(string id, string oldPsd, string newPsd)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.ChgPsd(id, oldPsd, newPsd));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage GetGoodBySubBar(string barcode)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.GetGoodBySubBar(barcode));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }

        [HttpGet]
        public HttpResponseMessage ChgGoodBySubBar(string barcode, string goodName, string unit, string subUnit, string subQty, string id)
        {
            string strJson = "";
            try
            {
                strJson = JsonConvert.SerializeObject(BasicBLL.ChgGoodBySubBar(barcode, goodName, unit, subUnit, subQty, id));
            }
            catch (Exception ex)
            {
                return ControllerFeedback.ExJson(ex);
            }
            return ControllerFeedback.OKJson(strJson);
        }
    }
}