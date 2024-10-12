using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using zym_api.BLL;
using zym_api.Models;
using static zym_api.Models.AddressModel;

namespace zym_api.Controllers
{
    public class AddressController: ApiController
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
        public IHttpActionResult InsertAddressInfo(string OpenId,string Name, string Phone, string Region,string Address,string Status, string Id)
        {
            try
            {
                string errMsg = "";
                string result = AddressBLL.InsertAddressInfo(OpenId, Name, Phone, Region, Address, Status,Id, out errMsg);
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
        public IHttpActionResult GetAddressInfo(string OpenId)
        {
            try
            {
                string errMsg = "";
                List<AddressList> list = AddressBLL.GetAddressInfo(OpenId, out errMsg);
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
        public IHttpActionResult DeleteAdderss(string OpenId,string Id)
        {
            try
            {
                string errMsg = "";
                string result = AddressBLL.DeleteAdderss(OpenId,Id, out errMsg);
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
        public IHttpActionResult GetAddressTrue(string OpenId)
        {
            try
            {
                string errMsg = "";
                List<AddressList> list = AddressBLL.GetAddressTrue(OpenId, out errMsg);
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

       
    }
}