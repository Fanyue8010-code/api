using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Security.Policy;
using System.Web;
using System.Web.UI.WebControls;
using System.Xml.Linq;
using zym_api.DAL;
using zym_api.Helper;
using zym_api.Models;

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

        public static int SaveGoodBasic(GoodBasic entity)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.SaveGoodBasic(entity));
            return i;
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
            return dt;
        }
        public static int DelGood(string id)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.DelGood(id));
            return i;
        }

        public static DataTable GetGoodByBarcode(string barcode)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetGoodByBarcode(barcode));
            if(dt.Rows.Count == 0)
            {
                throw new Exception("商品不存在");
            }
            return dt;
        }
        public static int SaveShelf(string shelf, string goodID, string unit)
        {
            int i = SQLHelper.ExecuteNonQuery(SQL.SaveShelf(shelf, goodID, unit));
            return i;
        }

        public static int OffShelf(string shelf, string barcode)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetBasicOutByBarcode(barcode));
            if(dt.Rows.Count == 0)
            {
                dt = SQLHelper.ExecuteDataTable(SQL.GetBasicInByBarcode(barcode));
            }
            if(dt.Rows.Count == 0)
            {
                throw new Exception("此货架未找到对应商品");
            }
            string strID = dt.Rows[0]["ID"].ToString();
            string strUnit = dt.Rows[0]["Unit"].ToString();
            DataTable dtID = SQLHelper.ExecuteDataTable(SQL.GetShelfByBarcode(shelf, strID, strUnit));
            if(dtID.Rows.Count == 0)
            {
                throw new Exception("此货架未找到对应商品");
            }
            int i = SQLHelper.ExecuteNonQuery(SQL.OffShelfByID(dtID.Rows[0]["ID"].ToString()));
            return i;
        }

        public static DataTable GetShelf(string good, string barcode, string subBarcode, string shelf)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetShelf(good, barcode, subBarcode, shelf));
            if(dt.Rows.Count == 0)
            {
                throw new Exception("数据不存在");
            }
            return dt;
        }

        public static DataTable ReqGood(string query)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.ReqGood(query));
            if (dt.Rows.Count == 0)
            {
                throw new Exception("商品不存在");
            }
            return dt;
        }

        public static string GetSeq()
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.GetSeq());
            string strSeq = DateTime.Now.ToString("yyyyMMdd") + dt.Rows[0][0].ToString().PadLeft(3, '0');
            return strSeq;
        }

        public static Req SaveReq(string id, string goodID, string unit, string qty, string user)
        {
            DataTable dt = SQLHelper.ExecuteDataTable(SQL.CopmareReq(id, goodID, unit));
            if (dt.Rows.Count == 2)
            {
                int iTotal = (dt.Select("FLAG = 'TOTAL'").Length > 0) ? Convert.ToInt32(dt.Select("FLAG = 'TOTAL'")[0]["QTY"]) : 0;
                int iReq = (dt.Select("FLAG = 'REQ'").Length > 0) ? Convert.ToInt32(dt.Select("FLAG = 'REQ'")[0]["QTY"]) : 0;
                if (iTotal <= iReq)
                {
                    throw new Exception("您已添加了全部库存");
                }
            }
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
    }
}