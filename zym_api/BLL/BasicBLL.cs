using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Xml.Linq;
using zym_api.DAL;
using zym_api.Helper;
using zym_api.Models;
using System.Drawing;

namespace zym_api.BLL
{
    public class BasicBLL
    {
        public static List<Category> GetCategory(string category)
        {
            DataTable dtCategory = SQLHelper.ExecuteDataTable(SQL.GetCategory(category));
            Category entity = new Category();
            List<Category> list = dtCategory.AsEnumerable().Select(row => new Category
            {
                Cate = row.Field<string>("Category"),
                ID = row.Field<Guid>("ID").ToString()
            }).ToList();
            return list;
        }

        public static int SaveCategory(string category)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.SaveCategory(category));
            return i;
        }

        public static int ChgCategory(string category, string source)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.ChgCategory(category, source));
            return i;
        }
        public static int DelCategory(string category)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.DelCategory(category));
            return i;
        }

        public static string SaveGoodBasic(GoodBasic entity)
        {
            //if(!string.IsNullOrEmpty(entity.Picture))
            //{
            //    entity.Picture = "查看";
            //}
            int i = 0;
            string strID = "";
            //查看商品条码是否已经存在
            DataTable dtBarCode = SQLHelper.ExecuteDataTable(SQL.IsExistGood(entity.Barcode));
            if(entity.Action == "A" && dtBarCode.Rows.Count > 0)
            {
                throw new Exception("商品条码已经存在于大包装。商品名：" + dtBarCode.Rows[0][0].ToString());
            }
            DataTable dtSubBarCode = SQLHelper.ExecuteDataTable(SQL.IsExistGoodSubBarcode(entity.Barcode));
            if (entity.Action == "A" && dtBarCode.Rows.Count > 0)
            {
                throw new Exception("商品条码已经存在于小包装。商品名：" + dtSubBarCode.Rows[0][0].ToString());
            }

            //有小包装
            if(entity.Action == "A" && entity.HasSubPack != "N")
            {
                dtBarCode = SQLHelper.ExecuteDataTable(SQL.IsExistGood(entity.SubPackBarcode));
                if (dtBarCode.Rows.Count > 0)
                {
                    throw new Exception("小商品条码已经存在于大包装。商品名：" + dtBarCode.Rows[0][0].ToString());
                }
                dtSubBarCode = SQLHelper.ExecuteDataTable(SQL.IsExistGoodSubBarcode(entity.SubPackBarcode));
                if (dtBarCode.Rows.Count > 0)
                {
                    throw new Exception("小商品条码已经存在于小包装。商品名：" + dtSubBarCode.Rows[0][0].ToString());
                }
            }

            if (entity.Action == "A")
            {
                i = SQLHelper.ExecuteNonQuery(SQL.SaveGoodBasic(entity));
                strID = entity.ID.ToString();
            }
            else
            {
                i = SQLHelper.ExecuteNonQuery(SQL.ChgGoodBasic(entity));
                strID = entity.GoodID.ToString();
            }
            if (!string.IsNullOrEmpty(entity.Picture))
            {
                byte[] imageBytes = Convert.FromBase64String(entity.Picture.Split(',')[1]);
                System.IO.File.WriteAllBytes("C:\\inetpub\\wwwroot\\ftp\\GoodImg\\" + strID + ".jpg", imageBytes);
                //using (var ms = new MemoryStream(imageBytes, 0, imageBytes.Length))
                //{
                //    // 从内存流创建Image
                //    using (Image image = Image.FromStream(ms))
                //    {
                //        // 保存图片到指定路径
                //        image.Save("C:\\inetpub\\wwwroot\\ftp\\GoodImg\\" + strID + ".jpg");
                //    }
                //}
            }
            return strID;
        }

        public static DataTable GetGoodByName(string name)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetGoodByName(name));
            if(dt.Rows.Count > 0)
            {
                throw new Exception("商品已经存在");
            }
            return dt;
            //添加商品，根据名称或者条码判断是否已经存在，
        }

        public static DataTable GetGood(string category, string goodName, string barcode, string subBarcode)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetGood(category, goodName, barcode, subBarcode));
            if (dt.Rows.Count == 0)
            {
                throw new Exception("商品不存在");
            }
            string strIP = ConfigurationManager.AppSettings["IP"].ToString();
            DataTable dtPath = SQLHelper.ExecuteDataTable(SQL.GetPath("GoodBasicPage"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strPicture = dt.Rows[i]["Picture"].ToString();
                string strPath = "";
                if (string.IsNullOrEmpty(strPicture))
                {
                    strPath = "https://" + strIP + "/Attach/" + dtPath.Rows[0][0].ToString() + "/default.jpg";
                }
                else
                {
                    string strGoodID = dt.Rows[i]["ID"].ToString();
                    strPath = "https://" + strIP + "/Attach/" + dtPath.Rows[0][0].ToString() + "/" + strGoodID + ".jpg";
                    Log.WriteLog(strPath);
                }
                dt.Rows[i]["Picture"] = strPath;
            }
            return dt;
        }
        public static int DelGood(string id)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.DelGood(id));
            return i;
        }

        public static DataTable GetGoodByBarcode(string shelf, string barcode)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetGoodByBarcode(barcode));
            if(dt.Rows.Count == 0)
            {
                throw new Exception("商品不存在");
            }
            string strGoodID = dt.Rows[0]["ID"].ToString();
            string strPack = dt.Rows[0]["PACK"].ToString();
            string strUnit = dt.Rows[0]["PackUnit"].ToString();
            string strPackBarcode = dt.Rows[0]["packbarcode"].ToString();
            string strIsHasSub = dt.Rows[0]["ISHASSUBPACK"].ToString();
            string strSubQty = dt.Rows[0]["subpackqty"].ToString();
            string strSubUnit = dt.Rows[0]["subpackunit"].ToString();
            string strSubBarcode = dt.Rows[0]["subpackbarcode"].ToString();
            if (strPack == "OUT" && strIsHasSub == "Y" && strSubQty != "0" && !string.IsNullOrEmpty(strSubUnit))
            {
                if (!string.IsNullOrEmpty(strSubBarcode) && strPackBarcode != strSubBarcode)
                {
                    for (int i = 0; i < Convert.ToInt32(strSubQty); i++)
                    {
                        SaveShelf(shelf, strGoodID, strSubUnit, true);
                    }
                }
                else
                {
                    throw new Exception("当前扫描的是整袋还是小包?");
                }
            }
            else if(strPack == "IN")
            {
                SaveShelf(shelf, strGoodID, strSubUnit, true);
            }
            else if(strPack == "OUT" && strIsHasSub == "N")
            {
                SaveShelf(shelf, strGoodID, strUnit, true);
            }
            return dt;
        }
        public static int SaveShelf(string shelf, string barcode, string unit, bool isApi = false)
        {
            int i = 0;
            if (isApi)
            {
                SQLHelper.ExecuteNonQuery(SQL.SaveShelf(shelf, barcode, unit));
                return i;
            }
            //找GoodID
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetGoodByBarcode(barcode));
            string strID = dt.Rows[0]["ID"].ToString();
            string strSubQty = dt.Rows[0]["subpackqty"].ToString();
            if(unit == "小包")
            {
                i = SQLHelper.ExecuteNonQuery(SQL.SaveShelf(shelf, dt.Rows[0]["ID"].ToString(), "小包"));
            }
            else
            {
                for(int j = 0; j < Convert.ToInt32(strSubQty); j++)
                {
                    i += SQLHelper.ExecuteNonQuery(SQL.SaveShelf(shelf, dt.Rows[0]["ID"].ToString(), "小包"));
                }
            }
            return i;
        }

        public static int OffShelfOutPack(string pack, string shelf, string barcode)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetBasicOutByBarcode(barcode));
            if (dt.Rows.Count == 0)
            {
                throw new Exception("未找到对应商品");
            }
            string strID = dt.Rows[0]["ID"].ToString();
            string strUnit = dt.Rows[0]["Unit"].ToString();
            int iSubQty = Convert.ToInt32(dt.Rows[0]["SubPackQty"]);
            DataTable dtID = SQLHelper.ExecuteDataTable(SQL.GetShelfByBarcode(shelf, strID));
            if (dtID.Rows.Count == 0)
            {
                throw new Exception("此货架未找到对应商品");
            }
            int i = 0;
            if (pack == "整袋")
            {
                if (dtID.Rows.Count < iSubQty)
                {
                    throw new Exception("此货架商品不足1" + strUnit);
                }
                for (int j = 0; j < iSubQty; j++)
                {
                    string strShelfID = dtID.Rows[j]["ID"].ToString();
                    i += SQLHelper.ExecuteNonQuery(SQL.OffShelfByID(strShelfID));
                }
            }
            else if(pack == "小包")
            {
                string strShelfID = dtID.Rows[0]["ID"].ToString();
                i += SQLHelper.ExecuteNonQuery(SQL.OffShelfByID(strShelfID));
            }
            return i;
        }

        public static int OffShelf(string shelf, string barcode)
        {
            string strWhere = "OUT";
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetBasicOutByBarcode(barcode));
            if(dt.Rows.Count == 0)
            {
                strWhere = "IN";
                dt = SQLHelper.ExecuteDataTable(SQL.GetBasicInByBarcode(barcode));
            }
            if(dt.Rows.Count == 0)
            {
                throw new Exception("未找到对应商品");
            }
            string strID = dt.Rows[0]["ID"].ToString();
            string strUnit = dt.Rows[0]["Unit"].ToString();
            int iSubQty = Convert.ToInt32(dt.Rows[0]["SubPackQty"]);
            int i = 0;
            if(strWhere == "OUT" && strUnit == "整袋")
            {
                throw new Exception("当前扫描的是整袋还是小包?");
            }
            if (iSubQty == 0 || strWhere == "IN")
            {
                DataTable dtID = SQLHelper.ExecuteDataTable(SQL.GetShelfByBarcode(shelf, strID, strUnit));
                if (dtID.Rows.Count == 0)
                {
                    throw new Exception("此货架未找到对应商品");
                }
                i = SQLHelper.ExecuteNonQuery(SQL.OffShelfByID(dtID.Rows[0]["ID"].ToString()));
            }
            else
            {
                DataTable dtID = SQLHelper.ExecuteDataTable(SQL.GetShelfByBarcode(shelf, strID));
                if(dtID.Rows.Count < iSubQty)
                {
                    throw new Exception("此货架商品不足1" + strUnit);
                }
                for(int j = 0; j < iSubQty; j++)
                {
                    string strShelfID = dtID.Rows[j]["ID"].ToString();
                    i += SQLHelper.ExecuteNonQuery(SQL.OffShelfByID(strShelfID));
                }
            }
            return i;
        }

        public static DataTable GetShelf(string good, string barcode, string subBarcode, string shelf)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetShelf(good, barcode, subBarcode, shelf));
            if(dt.Rows.Count == 0)
            {
                throw new Exception("数据不存在");
            }

            DataTable dtShelf = new DataTable();
            dtShelf.Columns.Add("GoodID");
            dtShelf.Columns.Add("GoodName");
            dtShelf.Columns.Add("qty");
            dtShelf.Columns.Add("GoodUnit");
            dtShelf.Columns.Add("Shelf");

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strGoodID = dt.Rows[i]["GoodID"].ToString();
                string strGood = dt.Rows[i]["GoodName"].ToString();
                int iCurrentQty = Convert.ToInt32(dt.Rows[i]["qty"]);
                int iSubQty = Convert.ToInt32(dt.Rows[i]["subpackqty"]);
                int iOut = 0;
                if(iCurrentQty >= iSubQty)
                {
                    if(iSubQty == 0)
                    {
                        dtShelf.Rows.Add(strGoodID, strGood, iCurrentQty, dt.Rows[i]["GoodUnit"].ToString(), dt.Rows[i]["Shelf"].ToString());
                        continue;
                    }
                    iOut = Convert.ToInt32(Convert.ToDouble(iCurrentQty / iSubQty));
                    if(iOut > 0)
                    {
                        DataRow dr = dtShelf.NewRow();
                        dr["GoodID"] = strGoodID;
                        dr["GoodName"] = strGood;
                        dr["qty"] = iOut.ToString();
                        dr["GoodUnit"] = dt.Rows[i]["packunit"].ToString();
                        dr["Shelf"] = dt.Rows[i]["Shelf"].ToString();
                        dtShelf.Rows.Add(dr);

                        if(iSubQty * iOut != iCurrentQty)
                        {
                            dtShelf.Rows.Add(strGoodID, strGood, iCurrentQty - (iSubQty * iOut), dt.Rows[i]["GoodUnit"].ToString(), dt.Rows[i]["Shelf"].ToString());
                        }
                    }
                }
                else
                {
                    DataRow dr = dtShelf.NewRow();
                    dr["GoodID"] = strGoodID;
                    dr["GoodName"] = strGood;
                    dr["qty"] = dt.Rows[i]["qty"].ToString();
                    dr["GoodUnit"] = dt.Rows[i]["goodunit"].ToString();
                    dr["Shelf"] = dt.Rows[i]["Shelf"].ToString();
                    dtShelf.Rows.Add(dr);
                }
            }
            return dtShelf;
        }

        public static DataTable ReqGood(string reqID, string query)
        {
            DataTable dtShelf = SQLHelper.ExecuteDataTable(SQL.GetReqGood(query));
            if (dtShelf.Rows.Count == 0)
            {
                throw new Exception("货架上未找到此商品");
            }
            //DataTable dt = SQLHelper.ExecuteDataTable(SQL.ReqGood(query));
            //if (dt.Rows.Count == 0)
            //{
            //    throw new Exception("商品不存在");
            //}
            //DataTable dtShelf = new DataTable();
            //dtShelf.Columns.Add("GoodID");
            //dtShelf.Columns.Add("GoodName");
            //dtShelf.Columns.Add("Qty");
            //dtShelf.Columns.Add("GoodUnit");

            //for (int i = 0; i < dt.Rows.Count; i++)
            //{
            //    string strGoodID = dt.Rows[i]["GoodID"].ToString();
            //    string strGood = dt.Rows[i]["GoodName"].ToString();
            //    int iCurrentQty = Convert.ToInt32(dt.Rows[i]["qty"]);
            //    int iSubQty = Convert.ToInt32(dt.Rows[i]["subpackqty"]);
            //    int iOut = 0;
            //    if (iCurrentQty >= iSubQty)
            //    {
            //        if (iSubQty == 0)
            //        {
            //            dtShelf.Rows.Add(strGoodID, strGood, iCurrentQty, dt.Rows[i]["GoodUnit"].ToString());
            //            continue;
            //        }
            //        iOut = Convert.ToInt32(Convert.ToDouble(iCurrentQty / iSubQty));
            //        if (iOut > 0)
            //        {
            //            DataRow dr = dtShelf.NewRow();
            //            dr["GoodID"] = strGoodID;
            //            dr["GoodName"] = strGood;
            //            dr["Qty"] = iOut.ToString();
            //            dr["GoodUnit"] = dt.Rows[i]["packunit"].ToString();
            //            dtShelf.Rows.Add(dr);

            //            if (iSubQty * iOut != iCurrentQty)
            //            {
            //                dtShelf.Rows.Add(strGoodID, strGood, iCurrentQty - (iSubQty * iOut), dt.Rows[i]["GoodUnit"].ToString());
            //            }
            //        }
            //    }
            //}
            //for(int i = 0; i < dtShelf.Rows.Count; i++)
            //{
            //    SQLHelper.ExecuteNonQuery(SQL.SaveReqTmp(reqID, dtShelf.Rows[i]["GOODID"].ToString(), dtShelf.Rows[i]["Qty"].ToString(), dtShelf.Rows[i]["GoodUnit"].ToString()));
            //}
            return dtShelf;
        }

        public static string GetSeq()
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetSeq());
            string strSeq = DateTime.Now.ToString("yyyyMMdd") + dt.Rows[0][0].ToString().PadLeft(3, '0');
            return strSeq;
        }

        public static Req SaveReq(string id, string goodID, string unit, string qty, string user)
        {
            //DataTable dt = SQLHelper.ExecuteDataTable(SQL.CopmareReq(id, goodID, unit));
            //if (dt.Rows.Count == 2)
            //{
            //    int iTotal = (dt.Select("FLAG = 'TOTAL'").Length > 0) ? Convert.ToInt32(dt.Select("FLAG = 'TOTAL'")[0]["QTY"]) : 0;
            //    int iReq = (dt.Select("FLAG = 'REQ'").Length > 0) ? Convert.ToInt32(dt.Select("FLAG = 'REQ'")[0]["QTY"]) : 0;
            //    if (iTotal <= iReq)
            //    {
            //        throw new Exception("您已添加了全部库存");
            //    }
            //}
            int i = SQLHelper.ExecuteNonQuery(SQL.SaveReq(id, goodID, unit, qty, user));
            Req entity = new Req();
            entity = GetReqInfo(id);
            return entity;
        }

        public static Req GetReqInfo(string reqNo)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetReqInfo(reqNo));
            Req entity = new Req();
            try
            {
                entity.Rows = Convert.ToInt32(dt.Compute("SUM(QTY)", string.Empty));
                entity.ReqList = dt;
            }
            catch
            {
                entity.Rows = 0;
                entity.ReqList = new DataTable();
            }
            return entity;
        }

        public static int DelReq(string reqNo, string goodID, string unit)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.DelReq(reqNo, goodID, unit));
            return i;
        }

        public static MenuItems GetMenu()
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetMenu());
            DataView dv = dt.DefaultView;
            DataTable dtGroup = dv.ToTable(true, new string[] { "PageGroup", "Icon" });
            MenuItems items = new MenuItems();
            for (int i = 0; i < dtGroup.Rows.Count; i++)
            {
                MenuList menu = new MenuList();
                string strGroup = dtGroup.Rows[i]["PageGroup"].ToString();
                DataRow[] drs = dt.Select("PageGroup = '"+strGroup+"'");
                menu.title = strGroup;
                menu.action = dtGroup.Rows[i]["Icon"].ToString();
                foreach (DataRow dr in drs)
                {
                    Item item = new Item();
                    item.title = dr["PageName"].ToString();
                    item.link = dr["PageLink"].ToString();
                    menu.items.Add(item);
                }
                items.menuList.Add(menu);
            }
            return items;
        }

        public static int ConfirmReq(string reqNo)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.ConfirmReq(reqNo));
            if(i == 0)
            {
                throw new Exception("没有找到叫货信息");
            }
            return i;
        }

        public static int DelReqNo(string reqNo)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.DelReqNo(reqNo));
            if (i == 0)
            {
                throw new Exception("没有找到叫货信息");
            }
            return i;
        }

        public static DataTable GetReqNo()
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetReqNo());
            if (dt.Rows.Count == 0)
            {
                throw new Exception("没有叫货单");
            }
            return dt;
        }

        public static DataTable GetShelf(string unit, string goodID)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetShelf(unit, goodID));
            if (dt.Rows.Count == 0)
            {
                throw new Exception("没找到此货物的货架信息");
            }
            return dt;
        }

        public static DataTable GetGoodByID(string id, string unit, string qty)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetGoodByID(id));
            if(dt.Rows.Count == 0)
            {
                throw new Exception("商品不存在");
            }
            string strPackUnit = dt.Rows[0]["PackUnit"].ToString();
            string strSubPackUnit = dt.Rows[0]["SubPackUnit"].ToString();
            string strHasSub = dt.Rows[0]["IsHasSubPack"].ToString();
            string strSubQty = dt.Rows[0]["SubPackQty"].ToString();
            if(unit == strPackUnit && strHasSub == "Y")
            {
                return SQLHelper.ExecuteDataTable(SQL.GetShelfByQty(strSubPackUnit, id, Convert.ToInt32(strSubQty) * Convert.ToInt32(qty)));
            }
            else if(unit == strPackUnit && strHasSub == "N")
            {
                return SQLHelper.ExecuteDataTable(SQL.GetShelf(strPackUnit, id));
            }
            else if(unit == strSubPackUnit)
            {
                return SQLHelper.ExecuteDataTable(SQL.GetShelf(strSubPackUnit, id));
            }
            return new DataTable();
        }

        public static bool IsShelfExist(string shelf)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.IsShelfExist(shelf));
            if (dt.Rows.Count == 0)
            {
                throw new Exception("扫描货架不存在");
            }
            return true;
        }

        public static bool CheckScanGood(string reqNo, string barcode, string shelf)
        {
            string strUnit = "";
            string strPack = "";
            string strID = "";
            string strPackBar = "";
            string strSubPackBar = "";
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetOutPack(barcode));
            
            if(dt.Rows.Count == 0)
            {
                dt = SQLHelper.ExecuteDataTable(SQL.GetInPack(barcode));
            }
            if(dt.Rows.Count == 0)
            {
                throw new Exception("商品条码扫描错误");
            }

            strID = dt.Rows[0]["ID"].ToString();
            strUnit = dt.Rows[0]["UNIT"].ToString();
            strPack = dt.Rows[0]["PACK"].ToString();
            strPackBar = dt.Rows[0]["PackBarcode"].ToString();
            strSubPackBar = dt.Rows[0]["SubPackBarcode"].ToString();


            //扫描商品是否在货架中
            DataTable dtOnShelf = SQLHelper.ExecuteDataTable(SQL.GetOnShelf(strID, strUnit, shelf));
            if(dtOnShelf.Rows.Count == 0)
            {
                throw new Exception("扫描货架不存在此商品");
            }
            //是否在需求单中
            DataTable dtReq = SQLHelper.ExecuteDataTable(SQL.GetReqByID(strID, strUnit, reqNo));
            if(dtReq.Rows.Count == 0)
            {
                throw new Exception("叫货单不包含此商品");
            }

            int i = SQLHelper.ExecuteNonQuery(SQL.SavePrepare(reqNo, strID, "1", strUnit, shelf));
            i += OffShelf(shelf, barcode);
            //查看当前拣货状态
            DataTable dtPre = SQLHelper.ExecuteDataTable(SQL.GetReqAndPre(reqNo, strID, strUnit));
            if(dtPre.Rows.Count == 0)
            {
                throw new Exception("叫货单号异常");
            }
            DataRow[] drR = dtPre.Select("FLAG = 'R'");
            DataRow[] drP = dtPre.Select("FLAG = 'P'");
            int iRQty = Convert.ToInt32(drR[0]["qty"].ToString());
            int iPQty = Convert.ToInt32(drP[0]["qty"].ToString());
            if(iRQty == iPQty)
            {
                i += SQLHelper.ExecuteNonQuery(SQL.ChgPreStatus(reqNo, strID, strUnit));
            }
            if(iPQty >= iRQty)
            {
                throw new Exception("此商品已拣货完成");
            }
            

            return true;
        }

        public static string OpenPic(string id)
        {
            string strIP = ConfigurationManager.AppSettings["IP"].ToString();
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetPath("GoodBasicPage"));
            string strPath = "https://" + strIP + "/Attach/" + dt.Rows[0][0].ToString() + "/" + id + ".jpg";
            return strPath;
        }

        public static int SaveShopGood(string goodID, string quantity, string unit, string spec, string exp, string expUnit, string save, string price, string qy)
        {
            DataTable dt = GetShopGoodByID(goodID);
            if(dt.Rows.Count > 0)
            {
                throw new Exception("商品已经存在, 请在当前页面查询后进行修改.");
            }
            int i = SQLHelper.ExecuteNonQuery(SQL.SaveShopGood(goodID, quantity, unit, spec, exp, expUnit, save, price, qy));
            return i;
        }

        public static DataTable GetGoods()
        {
            return SQLHelper.ExecuteDataTable(SQL.GetGoods());
        }
        public static DataTable GetQY()
        {
            return SQLHelper.ExecuteDataTable(SQL.GetQY());
        }

        public static int SaveQY(string ID, string value)
        {
            return SQLHelper.ExecuteNonQuery(SQL.SaveQY(ID, value));
        }

        public static DataTable GetShopGoodByID(string id)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetShopGoodByID(id));
            string strIP = ConfigurationManager.AppSettings["IP"].ToString();
            DataTable dtPath = SQLHelper.ExecuteDataTable(SQL.GetPath("GoodBasicPage"));
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string strPicture = dt.Rows[i]["Picture"].ToString();
                string strPath = "";
                if (string.IsNullOrEmpty(strPicture))
                {
                    strPath = "";
                }
                else
                {
                    string strGoodID = dt.Rows[i]["GoodID"].ToString();
                    strPath = "https://" + strIP + "/Attach/" + dtPath.Rows[0][0].ToString() + "/" + strGoodID + ".jpg";
                }
                dt.Rows[i]["Picture"] = strPath;
            }
            return dt;
        }

        public static int UpdShopGoodByID(string id, string qty, string unit, string spec, string exp, string expUnit, string save, string price, string qy)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.UpdShopGoodByID(id, qty, unit, spec, exp, expUnit, save, price, qy));
            return i;
        }

        public static int ChgFlag(string id, string flag)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.ChgFlag(id, flag));
            return i;
        }

        public static bool CheckReqScan(string reqNo, string barcode, string shelf)
        {
            string strUnit = "";
            string strPack = "";
            string strID = "";
            string strPackBar = "";
            string strSubPackBar = "";
            string strSubUnit = "";
            int iSubQty = 0;
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetOutPack(barcode));

            if (dt.Rows.Count == 0)
            {
                dt = SQLHelper.ExecuteDataTable(SQL.GetInPack(barcode));
            }
            if (dt.Rows.Count == 0)
            {
                throw new Exception("商品条码扫描错误");
            }

            strID = dt.Rows[0]["ID"].ToString();
            strUnit = dt.Rows[0]["UNIT"].ToString();
            strPack = dt.Rows[0]["PACK"].ToString();
            strPackBar = dt.Rows[0]["PackBarcode"].ToString();
            strSubPackBar = dt.Rows[0]["SubPackBarcode"].ToString();
            strSubUnit = dt.Rows[0]["SubPackUnit"].ToString();
            try
            {
                iSubQty = Convert.ToInt32(dt.Rows[0]["SubPackQty"]);
            }
            catch(Exception ex)
            {

            }

            //扫描商品是否在货架中
            DataTable dtOnShelf = SQLHelper.ExecuteDataTable(SQL.GetOnShelf(strID, "", shelf));
            if (dtOnShelf.Rows.Count == 0)
            {
                throw new Exception("扫描货架不存在此商品");
            }

            //是否在需求单中
            DataTable dtReq = SQLHelper.ExecuteDataTable(SQL.GetReqByID(strID, "", reqNo));
            if (dtReq.Rows.Count == 0)
            {
                throw new Exception("叫货单不包含此商品");
            }
            if (dtReq.Rows[0]["RequestStatus"].ToString() == "F")
            {
                throw new Exception("此商品已拣货");
            }
            //需求总数
            int iNeedQty = Convert.ToInt32(dtReq.Rows[0]["QUANTITY"]);

            //因为叫货是按照最小包装为单位叫货，如果果扫描的是外包装，计算是否超过叫货数
            if(strPack == "OUT")
            {
                if(iSubQty > iNeedQty)
                {
                    throw new Exception("整箱的数量多余需求数量");
                }
                if (string.IsNullOrEmpty(strSubUnit))
                {
                    strSubUnit = strUnit;
                }
                if(iSubQty == 0)
                {
                    iSubQty = 1;
                }
            }
            else
            {
                iSubQty = 1;
            }

            //下架
            SQLHelper.ExecuteNonQuery(SQL.OffShelf(iSubQty, shelf, strID));
            //保存
            SQLHelper.ExecuteNonQuery(SQL.SavePrepare(reqNo, strID, iSubQty.ToString(), strSubUnit, shelf));
            //查看当前拣货状态
            DataTable dtPre = SQLHelper.ExecuteDataTable(SQL.GetReqAndPre(reqNo, strID, strSubUnit));
            if (dtPre.Rows.Count == 0)
            {
                throw new Exception("叫货单号异常");
            }
            DataRow[] drR = dtPre.Select("FLAG = 'R'");
            DataRow[] drP = dtPre.Select("FLAG = 'P'");
            int iRQty = Convert.ToInt32(drR[0]["qty"].ToString());
            int iPQty = Convert.ToInt32(drP[0]["qty"].ToString());
            if (iRQty == iPQty)
            {
                SQLHelper.ExecuteNonQuery(SQL.ChgPreStatus(reqNo, strID, ""));
            }
            if (iPQty >= iRQty)
            {
                throw new Exception("此商品已拣货完成");
            }
            return true;
        }

        public static DataTable GetSysUser(string id, string psd)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetSysUser(id, psd));
            if(dt.Rows.Count == 0)
            {
                throw new Exception("用户名或密码错误");
            }
            return dt;
        }

        public static int ChgPsd(string id, string oldPsd, string newPsd)
        {
            if(oldPsd == newPsd)
            {
                throw new Exception("新密码不能与旧密码相同");
            }
            string strNew = Helper.Helper.Base64(newPsd);
            return SQLHelper.ExecuteNonQuery(SQL.ChgPsd(id, strNew));
        }
        public static DataTable GetGoodBySubBar(string barcode)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetGoodBySubBar(barcode));
            return dt;
        }
        public static int ChgGoodBySubBar(string barcode, string goodName, string unit, string subUnit, string subQty, string id)
        {
            return SQLHelper.ExecuteNonQuery(SQL.ChgGoodBySubBar(barcode, goodName, unit, subUnit, subQty, id));
        }
    }
}