using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using static zym_api.Models.GoodModel;
using zym_api.DAL;
using zym_api.Helper;
using Newtonsoft.Json;
using System.Web.DynamicData;
using System.Web.UI.WebControls;
using zym_api.Models;
using System.Web.Http.Results;
using System.Xml.Linq;
using System.Drawing;
using System.Net;
using System.Configuration;

namespace zym_api.BLL
{
    public class GoodBLL
    {
        public static List<GoodCategory> GetGoodCategory(out string errMsg)
        {
            errMsg = "OK";
            List<GoodCategory> list = new List<GoodCategory>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetGoodCategory()))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        GoodCategory ubi = new GoodCategory();
                        ubi.Id = dr["ID"].ToString();
                        ubi.Category = dr["Category"].ToString();
                        ubi.Picture = dr["Picture"].ToString();
                        // 判断图片是否存在
                        if (!CheckImageExists(ubi.Picture))
                        {
                            ubi.Picture = "../../assets/_huabanfuben.png"; // 设置为备用图片
                        }

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
        private static bool CheckImageExists(string url)
        {
            try
            {
                var request = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(url);
                request.Method = "HEAD";
                using (var response = (System.Net.HttpWebResponse)request.GetResponse())
                {
                    return response.StatusCode == System.Net.HttpStatusCode.OK;
                }
            }
            catch
            {
                return false;
            }
        }
        public static List<GoodCategory> GetGoodBasic(string CategoryID, string SearchValue, out string errMsg, string OpenId)
        {
            errMsg = "OK";
            List<GoodCategory> list = new List<GoodCategory>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetGoodBasic(CategoryID, SearchValue, OpenId)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        GoodCategory ubi = new GoodCategory();
                        ubi.CartID = dr["CartID"].ToString();
                        ubi.GoodBasicID = dr["GoodBasicID"].ToString();
                        ubi.CategoryID = dr["CategoryID"].ToString();
                        ubi.ShopGoodID = dr["ShopGoodID"].ToString();
                        ubi.Category = dr["Category"].ToString();
                        ubi.GoodName = dr["GoodName"].ToString();
                        ubi.Picture = dr["Picture"].ToString();
                        if (!CheckImageExists(ubi.Picture))
                        {
                            ubi.Picture = "../../assets/_huabanfuben.png"; // 设置为备用图片
                        }
                        ubi.Price = dr["Price"].ToString();
                        ubi.GoodQty = dr["GoodQty"].ToString();
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
        public static string InsertCart(string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int dt = SQLHelper.ExecuteNonQuery(GoodDAL.InsertCart(OpenId, CategoryID, GoodBasicID, ShopGoodID, CategoryCheck, GoodCheck, GoodQty));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string GetCart(string OpenId, out string errMsg)
        {
            errMsg = "OK";
            //  List<Cart> list = new List<Cart>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetCart(OpenId)))
                {
                    var result = from row in dt.AsEnumerable()
                                 group row by new
                                 {
                                     ID = row.Field<Guid>("CategoryID"),
                                     //   Category = row.Field<string>("Category")
                                 } into grp
                                 select new
                                 {
                                     CategoryID = grp.Key.ID,
                                     Category = grp.First().Field<string>("Category"),
                                     CategoryCheck = grp.First().Field<Boolean>("CategoryCheck"),
                                     merchandises = grp.Select(r => new
                                     {
                                         CartID = r.Field<Guid>("CartID"),
                                         GoodCheck = r.Field<Boolean>("GoodCheck"),
                                         GoodBasicID = r.Field<Guid>("GoodBasicID"),
                                         ShopGoodID = r.Field<Guid>("ShopGoodID"),
                                         GoodName = r.Field<string>("GoodName"),
                                         Picture = r.Field<string>("Picture"),
                                         Price = r.Field<Double>("Price"),
                                         GoodQty = r.Field<int>("GoodQty"),

                                     }).ToList()
                                 };

                    //转为ISON字符串
                    //string jsonResult=JsonConvert.SerializeObject(result);
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;
            }
        }
        public static List<CartGoodSum> GetCartGoodSum(string OpenId, out string errMsg)
        {
            errMsg = "OK";
            List<CartGoodSum> list = new List<CartGoodSum>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetCartGoodSum(OpenId)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        CartGoodSum ubi = new CartGoodSum();
                        ubi.Total = dr["Price"].ToString();
                        ubi.TotalCount = dr["GoodQty"].ToString();
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
        public static string MinusAmount(string OpenId, string CratId, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.MinusAmount(OpenId, CratId)))
                {
                    int Qty = Convert.ToInt32(dt.Rows[0]["GoodQty"]);
                    if (Qty == 0)
                    {
                        int del = SQLHelper.ExecuteNonQuery(GoodDAL.DeleteCart(OpenId, CratId));
                    }
                    if (Qty > 0)
                    {
                        int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateCart(OpenId, CratId, Qty));
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
        public static string PlusAmount(string OpenId, string CratId, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.PlusAmount(OpenId, CratId)))
                {
                    int Qty = Convert.ToInt32(dt.Rows[0]["GoodQty"]);
                    if (Qty == 0)
                    {
                        int del = SQLHelper.ExecuteNonQuery(GoodDAL.DeleteCart(OpenId, CratId));
                    }
                    if (Qty > 0)
                    {
                        int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateCart(OpenId, CratId, Qty));
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
        public static string UpdateCategoryCheck(string OpenId, string CategoryID, string CategoryCheck, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateCategoryCheck(OpenId, CategoryID, CategoryCheck));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string UpdateGoodCheck(string OpenId, string CartID, string GoodCheck, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateGoodCheck(OpenId, CartID, GoodCheck));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string UpdateCheckedAll(string OpenId, string checkedAll, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateCheckedAll(OpenId, checkedAll));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string DelCart(string OpenId, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int Up = SQLHelper.ExecuteNonQuery(GoodDAL.DelCart(OpenId));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string GoodToCart(string CartID, string OpenId, string CategoryID, string GoodBasicID, string ShopGoodID, string CategoryCheck, string GoodCheck, string GoodQty, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int dt = SQLHelper.ExecuteNonQuery(GoodDAL.GoodToCart(CartID, OpenId, CategoryID, GoodBasicID, ShopGoodID, CategoryCheck, GoodCheck, GoodQty));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string GetCartGoodToPay(string OpenId, out string errMsg)
        {
            errMsg = "OK";
            List<GoodsPay> list = new List<GoodsPay>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetCartGoodToPay(OpenId)))
                {
                    var result = from row in dt.AsEnumerable()
                                 group row by new
                                 {
                                     ID = row.Field<Guid>("CategoryID"),
                                     //   Category = row.Field<string>("Category")
                                 } into grp
                                 select new
                                 {
                                     CategoryID = grp.Key.ID,
                                     Category = grp.First().Field<string>("Category"),
                                     CategoryCheck = grp.First().Field<Boolean>("CategoryCheck"),
                                     merchandises = grp.Select(r => new
                                     {
                                         CartID = r.Field<Guid>("CartID"),
                                         GoodCheck = r.Field<Boolean>("GoodCheck"),
                                         GoodBasicID = r.Field<Guid>("GoodBasicID"),
                                         ShopGoodID = r.Field<Guid>("ShopGoodID"),
                                         GoodName = r.Field<string>("GoodName"),
                                         Picture = r.Field<string>("Picture"),
                                         Price = r.Field<Double>("Price"),
                                         GoodQty = r.Field<int>("GoodQty"),

                                     }).ToList()
                                 };

                    //转为ISON字符串
                    //string jsonResult=JsonConvert.SerializeObject(result);
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;

            }
        }

        public static string InsertPayOrder(List<GoodsPayList> goodsPays, string OpenId, string Name, string Phone, string Region, string Address, out string errMsg,string OrderNumber, string Status)
        {
            errMsg = "OK";
            //List<GoodsPay> list = new List<GoodsPay>();
            try
            {
                foreach (GoodsPayList item in goodsPays)
                {
                    GoodsPayList ubi = new GoodsPayList();
                    //ubi.OpenId = OpenId;
                    // ubi.CartID = item.CartID;
                    // ubi.GoodCheck =item.GoodCheck;
                    // ubi.OrderNumber= "123456789";
                    //ubi.GoodBasicID =item.GoodBasicID;
                    ubi.CategoryID = item.CategoryID;
                    // ubi.ShopGoodID= item.ShopGoodID;
                    ubi.Category = item.Category;
                    //ubi.GoodName =item.GoodName;
                    //ubi.Picture =item.Picture;
                    //ubi.Price= item.Price;
                    //ubi.GoodQty =item.GoodQty;
                    foreach (GoodsPay Orders in item.merchandises)
                    {
                        GoodsPay ub = new GoodsPay();
                        ub.OpenId = OpenId;
                        ub.CartID = Orders.CartID;
                        ub.GoodCheck = Orders.GoodCheck;
                        ub.OrderNumber = OrderNumber;
                        ub.GoodBasicID = Orders.GoodBasicID;
                        ub.CategoryID = ubi.CategoryID;
                        ub.ShopGoodID = Orders.ShopGoodID;
                        ub.Category = ubi.Category;
                        ub.GoodName = Orders.GoodName;
                        ub.Picture = Orders.Picture;
                        // 判断图片是否存在
                        if (!CheckImageExists(ub.Picture))
                        {
                            ub.Picture = "../../assets/_huabanfuben.png"; // 设置为备用图片
                        }
                        ub.Price = Orders.Price;
                        ub.GoodQty = Orders.GoodQty == "" ? "1" : Orders.GoodQty ;
                        ub.Name = Name;
                        ub.Phone = Phone;
                        ub.Region = Region;
                        ub.Address = Address;
                         ub.Status = Status;
                        int insert = SQLHelper.ExecuteNonQuery(GoodDAL.InsertOrder(ub));
                        if (ub.Status == "待发货")
                        {
                            int Del = SQLHelper.ExecuteNonQuery(GoodDAL.DeleteCart(OpenId, ub.CartID));
                        }
                    }
                    // ubi.CreateTime =item.CartID;
                    //ubi.PayTime= item.CartID;
                    //ubi.ShipDate= item.CartID;
                    //ubi.CompletionTime =item.CartID;


                }


                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;

            }
        }
        public static string InsertPayOrder(List<GoodsPay> goodsPays, string OpenId, string Name, string Phone, string Region, string Address, out string errMsg,string OrderNumber, string Status)
        {
            errMsg = "OK";
            //List<GoodsPay> list = new List<GoodsPay>();
            try
            {
                foreach (GoodsPay Orders in goodsPays)
                {

                    GoodsPay ub = new GoodsPay();
                    ub.OpenId = OpenId;
                    ub.CartID = Orders.CartID;
                    ub.GoodCheck = Orders.GoodCheck;
                    ub.OrderNumber = OrderNumber;
                    ub.GoodBasicID = Orders.GoodBasicID;
                    ub.CategoryID = Orders.CategoryID;
                    ub.ShopGoodID = Orders.ShopGoodID;
                    ub.Category = Orders.Category;
                    ub.GoodName = Orders.GoodName;
                    ub.Picture = Orders.Picture;
                    // 判断图片是否存在
                    if (!CheckImageExists(ub.Picture))
                    {
                        ub.Picture = "../../assets/_huabanfuben.png"; // 设置为备用图片
                    }
                    ub.Price = Orders.Price;
                    ub.GoodQty = Orders.GoodQty == "" ? "1" : Orders.GoodQty;
                    ub.Name = Name;
                    ub.Phone = Phone;
                    ub.Region = Region;
                    ub.Address = Address;
                    ub.Status = Status;
                    int insert = SQLHelper.ExecuteNonQuery(GoodDAL.InsertOrder(ub));
                    if (ub.Status == "待发货")
                    {
                        int Del = SQLHelper.ExecuteNonQuery(GoodDAL.DeleteCart(OpenId, ub.CartID));
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

        public static string GetOrder(string OpenId, string ID,string menuTapCurrent,out string errMsg)
        {
            errMsg = "OK";
            List<GoodsPay> list = new List<GoodsPay>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetOrder(OpenId, ID,menuTapCurrent)))
                {
                    var result = from row in dt.AsEnumerable()
                                 group row by new
                                 {
                                     ID = row.Field<string>("OpenID"),
                                     OrderId = row.Field<string>("OrderNumber")
                                 } into grp
                                 select new
                                 {
                                     OrderNumber = grp.Key.OrderId,
                                     OpenID = grp.Key.ID,
                                     Status = grp.First().Field<string>("Status"),
                                     TotalPrice = grp.First().Field<double>("TotalPrice"),
                                     TotalCount = grp.First().Field<int>("TotalCount"),
                                     Name = grp.First().Field<string>("Name"),
                                     Phone = grp.First().Field<string>("Phone"),
                                     Region = grp.First().Field<string>("Region"),
                                     Address = grp.First().Field<string>("Address"),
                                     CreateTime =  grp.First().Field<string>("CreateTime"),
                                     PayTime = grp.First().Field<string>("PayTime"),
                                     ShipDate = grp.First().Field<string>("ShipDate"),
                                     CompletionTime =  grp.First().Field<string>("CompletionTime"),
                                     CancelTime = grp.First().Field<string>("CancelTime"),
                                     Goods = from g in grp
                                             group g by new
                                             {
                                                 Category = g.Field<string>("Category"),
                                                 CategoryID = g.Field<Guid>("CategoryID"),
                                             } into ggrp
                                             select new
                                             {
                                                 Category = ggrp.Key.Category,
                                                 CategoryID = ggrp.Key.CategoryID,
                                                 // Name = ggrp.Key.Name,
                                                 // Phone = ggrp.Key.Phone,
                                                 // Region = ggrp.Key.Region,
                                                 //  Address = ggrp.Key.Address,
                                                 Merchandises = ggrp.Select(r => new
                                                 {
                                                     CartID = r.IsNull("CartID") ? (Guid?)null : r.Field<Guid>("CartID"),
                                                     GoodBasicID = r.Field<Guid>("GoodBasicID"),
                                                     ShopGoodID = r.Field<Guid>("ShopGoodID"),
                                                     GoodName = r.Field<string>("GoodName"),
                                                     Picture = r.Field<string>("Picture"),
                                                     Price = r.Field<double>("Price"),
                                                     GoodQty = r.Field<string>("GoodQty"),
                                                     CreateTime = r.Field<string>("CreateTime"),
                                                     PayTime =r.Field<string>("PayTime"),
                                                     ShipDate = r.Field<string>("ShipDate"),
                                                     CompletionTime =  r.Field<string>("CompletionTime"),
                                                     CancelTime = r.Field<string>("CancelTime")
                                                 }).ToList()
                                             }
                                 };
                    //var result = from row in dt.AsEnumerable()
                    //             group row by new
                    //             {
                    //                 ID = row.Field<string>("OpenID"),
                    //                 OrderId = row.Field<string>("OrderNumber")
                    //             } into grp
                    //             select new
                    //             {
                    //                 OrderNumber = grp.Key.OrderId,
                    //                 OpenID = grp.Key.ID,
                    //                 TotalPrice = grp.First().Field<Double>("TotalPrice"),
                    //                 TotalCount = grp.First().Field<int>("TotalCount"),
                    //                 Category = grp.First().Field<string>("Category"),
                    //                 CategoryID = grp.First().Field<Guid>("CategoryID"),
                    //                 Name = grp.First().Field<string>("Name"),
                    //                 Phone = grp.First().Field<string>("Phone"),
                    //                 Region = grp.First().Field<string>("Region"),
                    //                 Address = grp.First().Field<string>("Address"),
                    //                 merchandises = grp.Select(r => new
                    //                 {
                    //                     CartID = r.IsNull("CartID") ? (Guid?)null : r.Field<Guid>("CartID"),
                    //                     //GoodCheck = r.Field<Boolean>("GoodCheck"),
                    //                     GoodBasicID = r.Field<Guid>("GoodBasicID"),
                    //                     ShopGoodID = r.Field<Guid>("ShopGoodID"),
                    //                     GoodName = r.Field<string>("GoodName"),
                    //                     Picture = r.Field<string>("Picture"),
                    //                     Price = r.Field<Double>("Price"),
                    //                     GoodQty = r.Field<string>("GoodQty"),
                    //                     CreateTime = r.IsNull("CreateTime") ? (DateTime?)null :r.Field<DateTime>("CreateTime"),
                    //                     PayTime =  r.IsNull("PayTime") ? (DateTime?)null :r.Field<DateTime>("PayTime"),
                    //                     ShipDate =  r.IsNull("ShipDate") ? (DateTime?)null : r.Field<DateTime>("ShipDate"),
                    //                     CompletionTime = r.IsNull("CompletionTime") ? (DateTime?)null : r.Field<DateTime>("CompletionTime"),

                    //                 }).ToList()
                    //             };

                    //转为ISON字符串
                    //string jsonResult=JsonConvert.SerializeObject(result);
                    return JsonConvert.SerializeObject(result);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;

            }
        }
        public static string CancelOrder(string OpenId, string OrderNumber, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int dt = SQLHelper.ExecuteNonQuery(GoodDAL.CancelOrder(OpenId, OrderNumber));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
    }
}