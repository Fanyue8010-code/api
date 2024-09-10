using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Web;
using System.Xml.Linq;
using zym_api.DAL;
using zym_api.Helper;
using static zym_api.Models.AddressModel;

namespace zym_api.BLL
{
    public class AddressBLL
    {
        public static string InsertAddressInfo(string OpenId, string Name, string Phone, string Region, string Address, string Status, string Id, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                if (Id != "" && Id != null)
                {
                    if (Status == "true")
                    {
                        using (DataTable dy = SQLHelper.ExecuteDataTable(AddressDAL.GetAddressInfoStatus(OpenId)))
                        {
                            if (dy.Rows.Count > 0)
                            {
                                int i = SQLHelper.ExecuteNonQuery(AddressDAL.UpdateStatus(OpenId));
                                int y = SQLHelper.ExecuteNonQuery(AddressDAL.UpdateAddressInfo(OpenId, Name, Phone, Region, Address, Status, Id));
                            }
                            else
                            {
                                int y = SQLHelper.ExecuteNonQuery(AddressDAL.UpdateAddressInfo(OpenId, Name, Phone, Region, Address, Status, Id));
                            }
                        }
                    }
                    else
                    {
                        int y = SQLHelper.ExecuteNonQuery(AddressDAL.UpdateAddressInfo(OpenId, Name, Phone, Region, Address, Status, Id));
                    }
                }
                else
                {
                    if (Status == "true")
                    {
                        using (DataTable dy = SQLHelper.ExecuteDataTable(AddressDAL.GetAddressInfoStatus(OpenId)))
                        {
                            if (dy.Rows.Count > 0)
                            {
                                int i = SQLHelper.ExecuteNonQuery(AddressDAL.UpdateStatus(OpenId));
                                int y = SQLHelper.ExecuteNonQuery(AddressDAL.InsertAddressInfo(OpenId, Name, Phone, Region, Address, Status));
                            }
                            else
                            {
                                int y = SQLHelper.ExecuteNonQuery(AddressDAL.InsertAddressInfo(OpenId, Name, Phone, Region, Address, Status));
                            }
                        }
                    }
                    else
                    {
                        int y = SQLHelper.ExecuteNonQuery(AddressDAL.InsertAddressInfo(OpenId, Name, Phone, Region, Address, Status));
                    }
                }
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static List<AddressList> GetAddressInfo(string openId, out string errMsg)
        {
            errMsg = "OK";
            List<AddressList> list = new List<AddressList>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(AddressDAL.GetAddressInfo(openId)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        AddressList ubi = new AddressList();
                        ubi.Id = dr["ID"].ToString();
                        ubi.Name = dr["Name"].ToString();
                        ubi.Phone = dr["Phone"].ToString();
                        ubi.Region = dr["Region"].ToString();
                        ubi.Address = dr["Address"].ToString();
                        ubi.Default = dr["Status"].ToString();
                        list.Add(ubi);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return list;
            }
        }

         public static string DeleteAdderss(string OpenId,string Id, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int i = SQLHelper.ExecuteNonQuery(AddressDAL.DeleteAdderss(OpenId, Id));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
         public static List<AddressList> GetAddressTrue(string openId, out string errMsg)
        {
            errMsg = "OK";
            List<AddressList> list = new List<AddressList>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(AddressDAL.GetAddressTrue(openId)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        AddressList ubi = new AddressList();
                        ubi.Id = dr["ID"].ToString();
                        ubi.Name = dr["Name"].ToString();
                        ubi.Phone = dr["Phone"].ToString();
                        ubi.Region = dr["Region"].ToString();
                        ubi.Address = dr["Address"].ToString();
                        ubi.Default = dr["Status"].ToString();
                        list.Add(ubi);
                    }
                    return list;
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return list;
            }
        }
    }
}