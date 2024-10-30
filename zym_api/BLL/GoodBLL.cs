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
using zym_api.Controllers;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;
using Org.BouncyCastle.Utilities;
using System.Web.SessionState;

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
                            ubi.Picture = "http://49.233.191.59/ftp/GoodImg/default.jpg";  // 设置为备用图片
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
                            var lastSlashIndex = ubi.Picture.LastIndexOf('/');
                            if (lastSlashIndex != -1)
                            {
                                ubi.Picture = ubi.Picture.Substring(0, lastSlashIndex + 1) + "default.jpg";
                            }
                            //  ubi.Picture = "http://49.233.191.59/ftp/GoodImg/default.jpg"; // 设置为备用图片
                        }
                        decimal Price = Convert.ToDecimal(dr["Price"]);
                        ubi.Price = Price;
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
                                         Price = r.Field<decimal>("Price"),
                                         GoodQty = r.Field<int>("GoodQty"),

                                     }).ToList()
                                 };
                    var updatedResult = result.Select(category => new
                    {
                        CategoryID = category.CategoryID,
                        Category = category.Category,
                        CategoryCheck = category.CategoryCheck,
                        merchandises = category.merchandises.Select(ub =>
                        {
                            var pictureUrl = ub.Picture;

                            if (!CheckImageExists(pictureUrl))
                            {
                                var lastSlashIndex = pictureUrl.LastIndexOf('/');
                                if (lastSlashIndex != -1)
                                {
                                    pictureUrl = ub.Picture.Substring(0, lastSlashIndex + 1) + "default.jpg";
                                }
                                //pictureUrl = "http://49.233.191.59/ftp/GoodImg/default.jpg";  // Set to default image
                            }
                            return new
                            {
                                ub.CartID,
                                ub.GoodCheck,
                                ub.GoodBasicID,
                                ub.ShopGoodID,
                                ub.GoodName,
                                Picture = pictureUrl,
                                ub.Price,
                                ub.GoodQty
                            };
                        }).ToList()
                    });
                    //转为ISON字符串
                    //string jsonResult=JsonConvert.SerializeObject(result);
                    return JsonConvert.SerializeObject(updatedResult);
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
                                         Price = r.Field<decimal>("Price"),
                                         GoodQty = r.Field<int>("GoodQty"),
                                     }).ToList()
                                 };
                    var updatedResult = result.Select(category => new
                    {
                        CategoryID = category.CategoryID,
                        Category = category.Category,
                        CategoryCheck = category.CategoryCheck,
                        merchandises = category.merchandises.Select(ub =>
                        {
                            var pictureUrl = ub.Picture;
                            if (!CheckImageExists(pictureUrl))
                            {
                                var lastSlashIndex = pictureUrl.LastIndexOf('/');
                                if (lastSlashIndex != -1)
                                {
                                    pictureUrl = ub.Picture.Substring(0, lastSlashIndex + 1) + "default.jpg";
                                }
                                //  pictureUrl = "http://49.233.191.59/ftp/GoodImg/default.jpg";  // Set to default image
                            }
                            return new
                            {
                                ub.CartID,
                                ub.GoodCheck,
                                ub.GoodBasicID,
                                ub.ShopGoodID,
                                ub.GoodName,
                                Picture = pictureUrl,
                                ub.Price,
                                ub.GoodQty
                            };
                        }).ToList()
                    });
                    //转为ISON字符串
                    //string jsonResult=JsonConvert.SerializeObject(result);
                    return JsonConvert.SerializeObject(updatedResult);
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return null;

            }
        }

        public static string InsertPayOrder(List<GoodsPayList> goodsPays, string OpenId, string Name, string Phone, string Region, string Address, out string errMsg, string OrderNumber, string Status)
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
                        //  判断图片是否存在
                        if (!CheckImageExists(ub.Picture))
                        {
                            var lastSlashIndex = ub.Picture.LastIndexOf('/');
                            if (lastSlashIndex != -1)
                            {
                                ub.Picture = ub.Picture.Substring(0, lastSlashIndex + 1) + "default.jpg";
                            }
                            // ub.Picture = "http://49.233.191.59/ftp/GoodImg/default.jpg"; // 设置为备用图片
                        }
                        ub.Price = Orders.Price;
                        ub.GoodQty = Orders.GoodQty == "" ? "1" : Orders.GoodQty;
                        ub.Name = Name;
                        ub.Phone = Phone;
                        ub.Region = Region;
                        ub.Address = Address;
                        ub.Status = Status;
                        int insert = SQLHelper.ExecuteNonQuery(GoodDAL.InsertOrder(ub));
                        if (ub.Status == "待发货" || ub.Status == "")
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
        public static string InsertPayOrder(List<GoodsPay> goodsPays, string OpenId, string Name, string Phone, string Region, string Address, out string errMsg, string OrderNumber, string Status)
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
                        var lastSlashIndex = ub.Picture.LastIndexOf('/');
                        if (lastSlashIndex != -1)
                        {
                            ub.Picture = ub.Picture.Substring(0, lastSlashIndex + 1) + "default.jpg";
                        }
                        //ub.Picture = "http://49.233.191.59/ftp/GoodImg/default.jpg"; // 设置为备用图片
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

        public static string GetOrder(string OpenId, string ID, string menuTapCurrent, out string errMsg)
        {
            errMsg = "OK";
            List<GoodsPay> list = new List<GoodsPay>();
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetOrder(OpenId, ID, menuTapCurrent)))
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
                                     TotalPrice = grp.First().Field<decimal?>("TotalPrice"),
                                     TotalCount = grp.First().Field<int>("TotalCount"),
                                     Name = grp.First().Field<string>("Name"),
                                     Phone = grp.First().Field<string>("Phone"),
                                     Region = grp.First().Field<string>("Region"),
                                     Address = grp.First().Field<string>("Address"),
                                     CreateTime = grp.First().Field<string>("CreateTime") == null ? "" : grp.First().Field<string>("CreateTime"),
                                     PayTime = grp.First().Field<string>("PayTime") == null ? "" : grp.First().Field<string>("PayTime"),
                                     ShipDate = grp.First().Field<string>("ShipDate") == null ? "" : grp.First().Field<string>("ShipDate"),
                                     CompletionTime = grp.First().Field<string>("CompletionTime") == null ? "" : grp.First().Field<string>("CompletionTime"),
                                     CancelTime = grp.First().Field<string>("CancelTime") == null ? "" : grp.First().Field<string>("CancelTime"),
                                     TotalPriceRefund = grp.First().Field<decimal?>("TotalPriceRefund"),
                                     TotalApplyPrice = grp.FirstOrDefault()?.Field<decimal?>("TotalApplyPrice"),
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
                                                 Merchandises = ggrp.Select(r =>
                                                 {
                                                     var picture = r.Field<string>("Picture");
                                                     if (!CheckImageExists(picture))
                                                     {
                                                         var lastSlashIndex = picture.LastIndexOf('/');
                                                         if (lastSlashIndex != -1)
                                                         {
                                                             picture = picture.Substring(0, lastSlashIndex + 1) + "default.jpg";
                                                         }
                                                     }

                                                     return new
                                                     {
                                                         ID = r.Field<Guid>("ID"),
                                                         Name = r.Field<string>("Name"),
                                                         Phone = r.Field<string>("Phone"),
                                                         Region = r.Field<string>("Region"),
                                                         Address = r.Field<string>("Address"),
                                                         Status = r.Field<string>("Status"),
                                                         OrderNumber = grp.Key.OrderId,
                                                         CartID = r.IsNull("CartID") ? (Guid?)null : r.Field<Guid>("CartID"),
                                                         GoodBasicID = r.Field<Guid>("GoodBasicID"),
                                                         ShopGoodID = r.Field<Guid>("ShopGoodID"),
                                                         GoodName = r.Field<string>("GoodName"),
                                                         Picture = picture,
                                                         Price = r.Field<decimal?>("Price"),
                                                         GoodQty = r.Field<int>("GoodQty"),
                                                         CreateTime = r.Field<string>("CreateTime"),
                                                         PayTime = r.Field<string>("PayTime"),
                                                         ShipDate = r.Field<string>("ShipDate"),
                                                         CompletionTime = r.Field<string>("CompletionTime"),
                                                         CancelTime = r.Field<string>("CancelTime"),
                                                         Count = r.Field<int>("Count"),
                                                         TotalPriceRefund = r.Field<decimal?>("TotalPriceRefund"),
                                                         ApplyPhone = r.Field<string>("ApplyPhone"),
                                                         ApplyReason = r.Field<string>("ApplyReason"),
                                                         ApplyType = r.Field<string>("ApplyType"),
                                                         ApplyPrice = r.Field<decimal?>("ApplyPrice"),
                                                         ApplyRemark = r.Field<string>("ApplyRemark"),
                                                     };
                                                 }).ToList()
                                             }
                                 };
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
        //public static string NoPayOrder(string OpenId, string OrderNumber, out string errMsg)
        //{
        //    errMsg = "OK";
        //    try
        //    {
        //        using (DataTable dt1 = SQLHelper.ExecuteDataTable(GoodDAL.GetNeedCancelOrder(OpenId, OrderNumber)))
        //        {
        //            foreach (DataRow dr in dt1.Rows)
        //            {
        //                string OrdersStatus = dr["Status"].ToString();
        //                if (OrdersStatus != "待付款")
        //                {
        //                    throw new Exception("此订单已经处于" + OrdersStatus + "请下拉刷新页面");
        //                }
        //            }
        //        }
        //     int dt = SQLHelper.ExecuteNonQuery(GoodDAL.NoPayOrder(OpenId, OrderNumber));
        //        return "OK";
        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;
        //        return errMsg;
        //    }
        //}
        public static string RefundOrder(string OpenId, string OrderNumber, out string errMsg)
        {
            errMsg = "OK";
            try
            {
                int dt = SQLHelper.ExecuteNonQuery(GoodDAL.RefundOrder(OpenId, OrderNumber));
                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }
        }
        public static string NoPayOrders(string OpenId, out string errMsg)
        {

            errMsg = "OK";
            try
            {
                using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.NoPayOrders(OpenId)))
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        // string value = dr["ColumnName"].ToString(); // 获取特定列的值
                        int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateOrderFlag(OpenId, dr["ShopGoodID"].ToString(), dr["OrderID"].ToString()));
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
        public static string ApplyRefund(string OpenId, string ID, string Phone, string Reason, string Type, string Price, string Remark, out string errMsg)
        {

            errMsg = "OK";
            try
            {
                int Up = SQLHelper.ExecuteNonQuery(GoodDAL.ApplyRefund(OpenId, ID, Phone, Reason, Type, Price, Remark));
                // using (DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.NoRefundOrders(OpenId)))
                // {
                //     foreach (DataRow dr in dt.Rows)
                //     {
                //         // string value = dr["ColumnName"].ToString(); // 获取特定列的值
                ////     string    status = await  RefundController.GetRefunStatus(dr["Out_refund_no"].ToString());
                //        // int Up = SQLHelper.ExecuteNonQuery(GoodDAL.UpdateOrderFlag(OpenId, dr["Out_refund_no"].ToString()));
                //     }

                return "OK";
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
                return errMsg;
            }

        }

        //public static string NoPayOrderCancel(string OpenId, string OrderNumber, out string errMsg)
        //{
        //    errMsg = "OK";
        //    try
        //    {
        //        int dt = SQLHelper.ExecuteNonQuery(GoodDAL.NoPayOrderCancel(OpenId, OrderNumber));
        //        return "OK";
        //    }
        //    catch (Exception ex)
        //    {
        //        errMsg = ex.Message;
        //        return errMsg;
        //    }
        //}

        public static DataTable GetOrderList(string orderNo, string status, string prepare, string start, string end)
        {
            DateTime dtStart = new DateTime();
            DateTime dtEnd = new DateTime();

            if (!string.IsNullOrEmpty(start) && !string.IsNullOrEmpty(end))
            {
                try
                {
                    dtStart = Convert.ToDateTime(start);
                    if (dtStart.Year == 1)
                    {
                        throw new Exception("开始时间选择错误");
                    }
                }
                catch
                {
                    throw new Exception("开始时间选择错误");
                }
                try
                {
                    dtEnd = Convert.ToDateTime(end);
                    if (dtEnd.Year == 1)
                    {
                        throw new Exception("结束时间选择错误");
                    }
                }
                catch
                {
                    throw new Exception("结束时间选择错误");
                }
            }

            DataTable dtOrder = SQLHelper.ExecuteDataTable(GoodDAL.GetOrder(orderNo, status, prepare, start, end));
            if(dtOrder.Rows.Count == 0)
            {
                throw new Exception("没有查询到信息");
            }
            return dtOrder;

            #region 通过Api获取的资料
            /*
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            DateTime dateToConvert = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, dtStart.Hour, dtStart.Minute, dtStart.Second, DateTimeKind.Utc);
            TimeSpan timeStart = dateToConvert.ToUniversalTime() - epoch;
            dateToConvert = new DateTime(dtStart.Year, dtStart.Month, dtStart.Day, 23, 59, 59, DateTimeKind.Utc);
            TimeSpan timeEnd = dateToConvert.ToUniversalTime() - epoch;

            string strApiUrl = ConfigurationManager.AppSettings["ApiBaseURL"].ToString();
            strApiUrl += "wxa/sec/order/get_order_list?access_token=" + LoginBLL.GetToken();
            OrderList order = new OrderList();
            order.pay_time_range.begin_time = (long)timeStart.TotalSeconds;
            order.pay_time_range.end_time = (long)timeEnd.TotalSeconds;
            order.order_state = 2;
            order.page_size = 2;
            var resp = JsonHelper.Post(strApiUrl,JsonConvert.SerializeObject(order));
            JArray array = JArray.Parse(resp.order_list.ToString());
            OrderResponse respList = new OrderResponse();
            foreach (JObject item in array)
            {
                foreach (JProperty jProperty in item.Properties())
                {
                    if(jProperty.Name.ToString() == "transaction_id")
                    {
                        respList.PaySerialNo = jProperty.Value.ToString();
                    }
                    else if (jProperty.Name.ToString() == "merchant_trade_no")
                    {
                        respList.ShopSerialNo = jProperty.Value.ToString();
                    }
                    else if (jProperty.Name.ToString() == "description")
                    {
                        respList.ShopSerialNo = jProperty.Value.ToString();
                    }
                }
            }

            List<DesOrderList> items = JsonConvert.DeserializeObject<List<DesOrderList>>(resp.order_list);
            return null;
            */
            #endregion
        }

        public static int ChgPrepare(string orderNo, string id)
        {
            return SQLHelper.ExecuteNonQuery(GoodDAL.ChgPrepare(orderNo, id));
        }

        public static DeliveryFee GetFee()
        {
            DataTable dt = SQLHelper.ExecuteDataTable(GoodDAL.GetFee());
            DeliveryFee entity = new DeliveryFee();
            try
            {
                entity.Fee = dt.Select("free = 'DeliveryFee'")[0]["fee"].ToString();
                entity.Free = dt.Select("free = 'FreeDeliveryFee'")[0]["fee"].ToString();
            }
            catch
            {

            }
            return entity;
        }

        public static int ChgFee(string fee, string free)
        {
            return SQLHelper.ExecuteNonQuery(GoodDAL.ChgFee(fee, free));
        }

        public static void GetOrderStatus(string strTransId)
        {
            string strApiUrl = ConfigurationManager.AppSettings["ApiBaseURL"].ToString();
            strApiUrl += "wxa/sec/order/get_order?access_token=" + LoginBLL.GetToken();

            OrderByTransId entity = new OrderByTransId();
            entity.transaction_id = strTransId;
            var resp = JsonHelper.Post(strApiUrl, JsonConvert.SerializeObject(entity));
            var obj = JObject.Parse(resp.order.ToString());
            if (obj.order_state.ToString() != "1")
            {
                string strStatus = Helper.Helper.OrderStatus(obj.order_state.ToString());
                throw new Exception(strTransId + strStatus);
            }
        }

        public static void Shipping(string strTransId)
        {

        }
    }
}