using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI.WebControls;
using zym_api.BLL;
using zym_api.Models;

namespace zym_api.DAL
{
    public class SQL
    {
        public static StringBuilder strBuilder;
        public static string GetCategory(string category)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, CATEGORY FROM GOODCATEGORY ");
            if (!string.IsNullOrEmpty(category))
            {
                strBuilder.Append("WHERE CATEGORY = '" + category + "' ");
            }
            strBuilder.Append("ORDER BY CATEGORY");
            return strBuilder.ToString();
        }

        public static string SaveCategory(string category) 
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO GOODCATEGORY ");
            strBuilder.Append("(ID, CATEGORY) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("('"+Guid.NewGuid()+"', '"+category+"')");
            return strBuilder.ToString();
        }

        public static string ChgCategory(string category,string source)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE GOODCATEGORY ");
            strBuilder.Append("SET CATEGORY = '"+category+"' WHERE CATEGORY = '"+ source + "'");
            return strBuilder.ToString();
        }

        public static string DelCategory(string category)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("DELETE FROM GOODCATEGORY ");
            strBuilder.Append("WHERE CATEGORY = '" + category + "'");
            return strBuilder.ToString();
        }

        public static string SaveGoodBasic(GoodBasic entity)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO GOODBASIC ");
            strBuilder.Append("(ID, CATEGORYID, GOODNAME, PACKUNIT, PACKBARCODE, ISHASSUBPACK, SUBPACKUNIT, SUBPACKBARCODE, SUBPACKQTY, PICTURE) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("(");
            strBuilder.Append("'" + entity.ID + "',");
            strBuilder.Append("'" + entity.CategoryID + "',");
            strBuilder.Append("'" + entity.Name + "',");
            strBuilder.Append("'" + entity.Unit + "',");
            strBuilder.Append("'" + entity.Barcode + "',");
            strBuilder.Append("'" + entity.HasSubPack + "',");
            if (entity.HasSubPack == "N")
            {
                strBuilder.Append("'',");
                strBuilder.Append("'',");
                strBuilder.Append("'',");
            }
            else
            {
                strBuilder.Append("'" + entity.SubPackUnit + "',");
                strBuilder.Append("'" + entity.SubPackBarcode + "',");
                strBuilder.Append("'" + entity.SubPackQty + "',");
            }
            strBuilder.Append("'" + entity.Picture + "'");
            strBuilder.Append(")");
            return strBuilder.ToString();
        }

        public static string ChgGoodBasic(GoodBasic entity)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE GoodBasic ");
            strBuilder.Append("SET [CategoryID] = '"+entity.CategoryID+"' ");
            strBuilder.Append(",[GoodName] = '"+entity.Name+"' ");
            strBuilder.Append(",[PackUnit] = '"+entity.Unit+"' ");
            strBuilder.Append(",[PackBarcode] = '"+entity.Barcode+"' ");
            strBuilder.Append(",[IsHasSubPack] = '"+entity.HasSubPack+"' ");
            if(entity.HasSubPack == "N")
            {
                strBuilder.Append(",[SubPackUnit] ='' ");
                strBuilder.Append(",[SubPackBarcode] = '' ");
                strBuilder.Append(",[SubPackQty] = '' ");
            }
            else
            {
                strBuilder.Append(",[SubPackUnit] ='" + entity.SubPackUnit + "' ");
                strBuilder.Append(",[SubPackBarcode] = '" + entity.SubPackBarcode + "' ");
                strBuilder.Append(",[SubPackQty] = '" + entity.SubPackQty + "' ");
            }
            strBuilder.Append(",[Picture] = '"+entity.Picture+"' ");
            strBuilder.Append("WHERE ID = '"+entity.GoodID+"' ");
            return strBuilder.ToString();
        }

        public static string GetGoodByName(string name)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID FROM GOODBASIC ");
            strBuilder.Append("WHERE GOODNAME = N'"+name+"' ");
            strBuilder.Append("AND FLAG = 'T'");
            return strBuilder.ToString();
        }

        public static string GetGood(string category, string goodName, string barcode, string subBarcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ");
            strBuilder.Append("C.Category, C.ID AS CATEID, B.ID, B.GoodName, B.PackUnit, B.PackBarcode, B.IsHasSubPack, B.SubPackUnit, B.SubPackBarcode, B.SubPackQty, B.Picture ");
            strBuilder.Append("FROM GoodBasic B ");
            strBuilder.Append("LEFT JOIN GOODCATEGORY C ");
            strBuilder.Append("ON B.CATEGORYID = C.ID ");
            strBuilder.Append("WHERE FLAG = 'T' ");
            if (!string.IsNullOrEmpty(category))
            {
                strBuilder.Append("AND C.ID = '"+category+"' ");
            }
            if (!string.IsNullOrEmpty(goodName))
            {
                strBuilder.Append("AND B.GOODNAME LIKE N'%"+goodName+"%' ");
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                strBuilder.Append("AND B.PACKBARCODE = '"+barcode+"' ");
            }
            if (!string.IsNullOrEmpty(subBarcode))
            {
                strBuilder.Append("AND B.SUBPACKBARCODE = '"+subBarcode+"' ");
            }
            return strBuilder.ToString();
        }

        public static string DelGood(string id)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE GoodBasic SET FLAG = 'F' WHERE ID = '"+ id + "'");
            return strBuilder.ToString();
        }

        public static string GetGoodByBarcode(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, GoodName, PackUnit,  ISHASSUBPACK, subpackqty, subpackunit, subpackbarcode,  packbarcode,'OUT' AS PACK FROM GOODBASIC where PackBarcode= '" + barcode + "' ");
            strBuilder.Append("UNION ALL ");
            strBuilder.Append("SELECT ID, GoodName, SubPackUnit, ISHASSUBPACK, subpackqty, subpackunit, subpackbarcode, packbarcode,'IN' AS PACK FROM GOODBASIC where SubPackBarcode= '" + barcode + "' ");
            return strBuilder.ToString();
        }

        public static string SaveShelf(string shelf, string goodID, string unit)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO GoodShelf ");
            strBuilder.Append("(ID, SHELF, GOODID, GOODUNIT) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("(");
            strBuilder.Append("'" + Guid.NewGuid() + "',");
            strBuilder.Append("'" + shelf.ToUpper() + "',");
            strBuilder.Append("'" + goodID + "',");
            strBuilder.Append("'" + unit + "'");
            strBuilder.Append(")");
            return strBuilder.ToString();
        }

        public static string GetShelfByBarcode( string shelf, string goodID, string unit)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("select ID from GoodShelf ");
            strBuilder.Append("WHERE GoodStatus = 'T' ");
            strBuilder.Append("AND SHELF = '"+shelf+"' ");
            strBuilder.Append("AND GOODID = '" + goodID + "' ");
            strBuilder.Append("AND GOODUNIT = '" + unit + "' ");
            return strBuilder.ToString();
        }

        public static string GetShelfByBarcode(string shelf, string goodID)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("select ID from GoodShelf ");
            strBuilder.Append("WHERE GoodStatus = 'T' ");
            strBuilder.Append("AND SHELF = '" + shelf + "' ");
            strBuilder.Append("AND GOODID = '" + goodID + "' ");
            return strBuilder.ToString();
        }

        public static string GetBasicOutByBarcode(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, PACKUNIT as Unit, SubPackQty FROM GoodBasic WHERE  FLAG = 'T' AND PackBarcode = '" + barcode+"'");
            return strBuilder.ToString();
        }

        public static string GetBasicInByBarcode(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, SUBPACKUNIT  as Unit, SubPackQty FROM GoodBasic WHERE  FLAG = 'T' AND SubPackBarcode = '" + barcode + "'");
            return strBuilder.ToString();
        }

        public static string OffShelfByID(string id)
        {
            strBuilder= new StringBuilder();
            strBuilder.Append("UPDATE GoodShelf SET GoodStatus = 'F' WHERE ID = '"+id+"'");
            return strBuilder.ToString();
        }

        public static string GetShelf(string good, string barcode, string subBarcode, string shelf)
        {
            strBuilder = new StringBuilder();
            //strBuilder.Append("select b.GoodName, sum(s.goodqty) as qty, s.GoodUnit, s.Shelf ");
            strBuilder.Append("select s.GoodID, b.GoodName, B.packunit, b.subpackunit, b.subpackqty, sum(s.goodqty) as qty, s.GoodUnit, s.Shelf ");
            strBuilder.Append("from GoodShelf s ");
            strBuilder.Append("LEFT JOIN GoodBasic B ");
            strBuilder.Append("ON S.GoodID = B.ID ");
            strBuilder.Append("WHERE S.GoodStatus = 'T' ");
            strBuilder.Append("AND B.FLAG = 'T' ");
            if (!string.IsNullOrEmpty(good))
            {
                strBuilder.Append("AND B.GoodName LIKE '%" + good.Trim() + "%' ");
            }
            if (!string.IsNullOrEmpty(barcode))
            {
                strBuilder.Append("AND PackBarcode = '" + barcode.Trim() + "' ");
            }
            if (!string.IsNullOrEmpty(subBarcode))
            {
                strBuilder.Append("AND SubPackBarcode = '" + subBarcode.Trim() + "' ");
            }
            if (!string.IsNullOrEmpty(shelf))
            {
                strBuilder.Append("AND Shelf = '" + shelf.Trim() + "' ");
            }
            strBuilder.Append("group by s.GoodID,b.GoodName, B.packunit, s.GoodUnit,b.subpackunit,b.subpackqty, s.Shelf ");
            strBuilder.Append("ORDER BY GoodName, GoodUnit, SHELF ");
            return strBuilder.ToString();
        }

        public static string IsTmp(string reqNo)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT COUNT(*) FROM GoodRequestTmp WHERE ID = '"+reqNo+"'");
            return strBuilder.ToString();
        }

        public static string ReqGood(string query)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT s.GoodID, b.GoodName, B.packunit, b.subpackunit, b.subpackqty, sum(s.goodqty) as qty, s.GoodUnit FROM GoodShelf S  ");
            strBuilder.Append("LEFT JOIN GoodBasic B  ");
            strBuilder.Append("ON S.GoodID = B.ID  ");
            strBuilder.Append("WHERE S.GoodStatus = 'T'  ");
            strBuilder.Append("AND B.FLAG = 'T'  ");
            strBuilder.Append("AND (B.GoodName LIKE '%"+ query + "%' OR PackBarcode = '"+ query + "' OR SubPackBarcode = '"+ query + "')  ");
            strBuilder.Append("group by s.GoodID, b.GoodName, B.packunit, b.subpackunit, b.subpackqty, s.GoodUnit ");
            return strBuilder.ToString();
        }
        
        public static string GetSeq()
        {
            string strDate = DateTime.Now.ToString("yyyy-MM-dd");
            strBuilder = new StringBuilder();
            strBuilder.Append("IF EXISTS( ");
            strBuilder.Append("SELECT * FROM GoodRequestSeq ");
            strBuilder.Append("WHERE DATESTAMP = '"+ strDate + "') ");
            strBuilder.Append("UPDATE GoodRequestSeq SET NextSeq = NextSeq + 1 ");
            strBuilder.Append("WHERE DATESTAMP = '"+ strDate + "' ");
            strBuilder.Append("ELSE ");
            strBuilder.Append("INSERT INTO GoodRequestSeq ");
            strBuilder.Append("(DateStamp, NEXTSEQ) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("('"+ strDate + "', 1) ");
            strBuilder.Append("SELECT NEXTSEQ FROM GoodRequestSeq ");
            strBuilder.Append("WHERE DATESTAMP = '"+ strDate + "'");
            return strBuilder.ToString();
        }

        public static string SaveReq(string id, string goodID, string unit, string qty, string user)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("DELETE FROM GoodRequest WHERE ID = '"+id+"' AND GOODID = '"+goodID+"';");
            strBuilder.Append("INSERT INTO GoodRequest ");
            strBuilder.Append("(ID, GOODID, QUANTITY, GOODUNIT, REQUESTUSER) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("(");
            strBuilder.Append("'" + id + "',");
            strBuilder.Append("'" + goodID + "',");
            strBuilder.Append("'" + qty + "',");
            strBuilder.Append("'" + unit + "',");
            strBuilder.Append("'" + user + "'");
            strBuilder.Append(")");
            return strBuilder.ToString();
        }

        public static string CopmareReq(string reqNo, string goodID, string unit)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("select isnull(SUM(Quantity), '0') AS QTY, 'REQ' AS FLAG ");
            strBuilder.Append("from [GoodRequest] ");
            strBuilder.Append("WHERE ID = '"+ reqNo + "' ");
            strBuilder.Append("AND RequestStatus = 'R' ");
            strBuilder.Append("AND GoodID = '"+ goodID + "' ");
            strBuilder.Append("and goodunit = '"+ unit + "' ");
            strBuilder.Append("UNION ALL ");
            strBuilder.Append("SELECT isnull(sum(Quantity), '0'), 'TOTAL' ");
            strBuilder.Append("FROM goodrequesttmp ");
            strBuilder.Append("WHERE ID = '"+reqNo+"' ");
            strBuilder.Append("AND GoodID = '" + goodID + "' ");
            strBuilder.Append("AND GoodUnit = '"+ unit + "'");
            return strBuilder.ToString();
        }

        public static string SaveReqTmp(string reqNo, string goodID, string qty, string unit)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO GoodRequestTmp ");
            strBuilder.Append("(ID, GOODID, QUANTITY, GOODUNIT) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("('"+reqNo+"','"+goodID+"', '"+qty+"', '"+unit+"')");
            return strBuilder.ToString();
        }

        public static string GetReqInfo(string reqNo)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT R.GoodID, B.GOODNAME, SUM(QUANTITY) AS QTY, R.GoodUnit,  ");
            strBuilder.Append("CASE WHEN R.RequestStatus = 'R' THEN '等待确认' WHEN R.RequestStatus = 'T' THEN '等待拣货' WHEN R.RequestStatus = 'F' THEN '完成' END AS FLAG, '' AS SHELF, ");
            strBuilder.Append("(SELECT ISNULL(SUM(QUANTITY), 0)  FROM GoodPrepare P WHERE ID = '"+reqNo+"' AND P.GoodID = R.GoodID AND P.GoodUnit = R.GoodUnit) AS PQTY ");
            strBuilder.Append("FROM GoodRequest R ");
            strBuilder.Append("LEFT JOIN GoodBasic B ");
            strBuilder.Append("ON R.GoodID = B.ID ");
            strBuilder.Append("WHERE R.ID = '"+reqNo+"' ");
            strBuilder.Append("AND B.FLAG = 'T' ");
            strBuilder.Append("GROUP BY R.GOODID,B.GOODNAME, R.GOODUNIT,R.RequestStatus ");
            return strBuilder.ToString();
        }

        public static string DelReq(string reqNo, string goodID, string unit)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("delete from GoodRequest ");
            strBuilder.Append("where ID = '"+reqNo+"' and goodid = '"+ goodID + "' and GoodUnit = N'"+ unit + "'");
            return strBuilder.ToString();
        }

        public static string GetMenu()
        {
            strBuilder  = new StringBuilder();
            strBuilder.Append("SELECT * FROM PAGELIST ORDER BY ID");
            return strBuilder.ToString();
        }

        public static string ConfirmReq(string reqNo)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE GoodRequest SET REQUESTSTATUS = 'T' WHERE ID = '"+reqNo+"'");
            return strBuilder.ToString();

        }

        public static string DelReqNo(string reqNo)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("DELETE FROM  GoodRequest WHERE ID = '" + reqNo + "'");
            return strBuilder.ToString();

        }

        public static string GetReqNo()
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("select DISTINCT ID from GoodRequest ");
            strBuilder.Append("WHERE RequestStatus= 'T' ");
            strBuilder.Append("ORDER BY ID ");
            return strBuilder.ToString();
        }

        public static string GetShelf(string unit, string goodID)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT DISTINCT SHELF FROM GoodShelf ");
            strBuilder.Append("WHERE GoodStatus = 'T' ");
            strBuilder.Append("AND GoodUnit = N'" + unit + "' ");
            strBuilder.Append("AND GoodID = '" + goodID + "' ");
            strBuilder.Append("ORDER BY SHELF ");
            return strBuilder.ToString();
        }
        public static string GetShelfByQty(string unit, string goodID, int subQty)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT DISTINCT SHELF FROM GoodShelf ");
            strBuilder.Append("WHERE GoodStatus = 'T' ");
            strBuilder.Append("AND GoodUnit = N'" + unit + "' ");
            strBuilder.Append("AND GoodID = '" + goodID + "' ");
            //strBuilder.Append("GROUP BY SHELF ");
            //strBuilder.Append("HAVING COUNT(*) >= "+ subQty +" ");
            strBuilder.Append("ORDER BY SHELF ");
            return strBuilder.ToString();
        }

        public static string GetGoodByID(string id)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT * FROM GOODBASIC WHERE  FLAG = 'T' AND ID = '"+ id + "' ");
            return strBuilder.ToString();
        }

        public static string IsShelfExist(string shelf)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID FROM GoodShelf ");
            strBuilder.Append("WHERE GoodStatus = 'T' ");
            strBuilder.Append("AND SHELF = '"+ shelf + "' ");
            return strBuilder.ToString();
        }

        public static string GetOutPack(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, PackUnit AS UNIT, subpackunit, 'OUT' AS PACK, PackBarcode, SubPackBarcode, SubPackQty FROM GoodBasic ");
            strBuilder.Append("WHERE FLAG = 'T' ");
            strBuilder.Append("AND PackBarcode = '"+ barcode + "' ");
            return strBuilder.ToString();
        }

        public static string GetInPack(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, PackUnit AS UNIT, subpackunit, 'IN' AS PACK, PackBarcode, SubPackBarcode, SubPackQty FROM GoodBasic ");
            strBuilder.Append("WHERE FLAG = 'T' ");
            strBuilder.Append("AND SubPackBarcode = '" + barcode + "' ");
            return strBuilder.ToString();
        }

        public static string GetOnShelf(string goodID, string unit, string shelf)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT GOODID FROM GoodShelf ");
            strBuilder.Append("WHERE GoodStatus = 'T' ");
            strBuilder.Append("AND SHELF = '"+shelf+"' ");
            strBuilder.Append("and goodid = '"+ goodID + "' ");
            if (!string.IsNullOrEmpty(unit))
            {
                strBuilder.Append("and GoodUnit = N'" + unit + "' ");
            }
            return strBuilder.ToString();
        }

        public static string GetReqByID(string goodID, string unit, string reqNo)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT QUANTITY, RequestStatus FROM GoodRequest ");
            strBuilder.Append("WHERE ID = '"+reqNo+"' ");
            strBuilder.Append("AND GOODID = '"+goodID+"' ");
            if (!string.IsNullOrEmpty(unit))
            {
                strBuilder.Append("AND GoodUnit = N'" + unit + "' ");
            }
            return strBuilder.ToString();
        }

        public static string GetReqAndPre(string reqNo, string goodID, string unit)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT sum(quantity) as qty, 'R' AS FLAG FROM GoodRequest ");
            strBuilder.Append(" where  RequestStatus = 'T'  ");
            strBuilder.Append("and id = '"+ reqNo + "' ");
            strBuilder.Append("and goodid = '"+ goodID + "' ");
            strBuilder.Append("and GoodUnit = N'"+ unit + "' ");
            strBuilder.Append("union all ");
            strBuilder.Append("SELECT ISNULL(SUM(QUANTITY), 0) AS QTY, 'P' FROM GoodPrepare ");
            strBuilder.Append("WHERE id = '"+ reqNo + "' ");
            strBuilder.Append("and goodid = '"+ goodID + "' ");
            strBuilder.Append("and GoodUnit = N'"+ unit + "' ");
            return strBuilder.ToString();
        }


        public static string SavePrepare(string reqNo,string goodID, string qty, string unit, string shelf)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO GoodPrepare ");
            strBuilder.Append("(ID, GOODID, QUANTITY, GOODUNIT, SHELF) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("(");
            strBuilder.Append("'" + reqNo + "',");
            strBuilder.Append("'" + goodID + "',");
            strBuilder.Append("'" + qty + "',");
            strBuilder.Append("'" + unit + "',");
            strBuilder.Append("'" + shelf + "'");
            strBuilder.Append(")");
            return strBuilder.ToString();
        }

        public static string ChgPreStatus (string reqNo, string goodID, string unit)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("update GoodRequest set RequestStatus = 'F' where id = '"+ reqNo + "' and goodid = '"+ goodID + "' ");
            if (!string.IsNullOrEmpty(unit))
            {
                strBuilder.Append(" and GoodUnit = '"+ unit + "' ");
            }
            return strBuilder.ToString();
        }

        public static string GetPath(string useFor)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT USEVALUE FROM SYSSETTING WHERE USEFOR = '"+useFor+"' AND USEKEY = 'ImgPath'");
            return strBuilder.ToString();
        }

        public static string SaveShopGood(string goodID, string quantity, string unit, string spec, string exp, string expUnit, string save, string price, string qy)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("INSERT INTO ShopGood ");
            strBuilder.Append("(ID, GOODID, QTY, UNIT, SPEC, Expdate ,ExpUnit,  SAVEMODE, PRICE, QY ) ");
            strBuilder.Append("VALUES ");
            strBuilder.Append("(");
            strBuilder.Append("'"+Guid.NewGuid()+"',");
            strBuilder.Append("'"+goodID+"',");
            strBuilder.Append("'" + quantity + "',");
            strBuilder.Append("'" + unit + "',");
            strBuilder.Append("'" + spec + "',");
            strBuilder.Append("'" + exp + "',");
            strBuilder.Append("'" + expUnit + "',");
            strBuilder.Append("'" + save + "',");
            strBuilder.Append("'" + price + "',");
            strBuilder.Append("'" + qy.Replace(",", "/")+ "'");
            strBuilder.Append(")");
            return strBuilder.ToString();
        }

        public static string GetGoods()
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, GoodName FROM GoodBasic WHERE FLAG = 'T' ORDER BY GOODNAME");
            return strBuilder.ToString();
        }

        public static string GetQY()
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT * FROM QY");
            return strBuilder.ToString();
        }

        public static string SaveQY(string ID, string value)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE QY ");
            strBuilder.Append("SET ");
            strBuilder.Append("KEYVALUE = '"+value+"' WHERE ID = '"+ID+"'");
            return strBuilder.ToString();
        }

        public static string GetShopGoodByID(string id)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT g.*, b.GoodName, b.Picture, (g.ExpDate + ' ' + g.ExpUnit) as Exp, CASE WHEN g.FLAG = 'Y' THEN '售卖中' ELSE '停止售卖' END AS STATUS FROM ShopGood g ");
            strBuilder.Append("left join GoodBasic b ");
            strBuilder.Append("on g.GoodID = b.id ");
            strBuilder.Append("where GoodID = '"+id+"' ");
            return strBuilder.ToString();
        }

        public static string UpdShopGoodByID(string id, string qty, string unit, string spec, string exp, string expUnit, string save, string price, string qy)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE ShopGood ");
            strBuilder.Append("SET QTY = '"+qty+"',");
            strBuilder.Append("UNIT = '"+unit+"',");
            strBuilder.Append("SPEC = '"+spec+"',");
            strBuilder.Append("EXPDATE = '"+exp+"',");
            strBuilder.Append("SAVEMODE = '"+save+"',");
            strBuilder.Append("PRICE = '"+price+"',");
            strBuilder.Append("QY = '"+qy.Replace(",", "/") + "',");
            strBuilder.Append("EXPUNIT = '"+expUnit+"' ");
            strBuilder.Append("WHERE ID = '"+id+"' ");
            return strBuilder.ToString();
        }

        public static string ChgFlag(string id, string flag)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE SHOPGOOD SET FLAG = '"+ flag + "' WHERE ID = '"+id+"'");
            return strBuilder.ToString();
        }

        public static string GetReqGood(string good)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("select s.GoodID, b.GoodName, sum(s.goodqty) as Qty, s.GoodUnit, '' AS NeedQty ");
            strBuilder.Append("from GoodShelf s  ");
            strBuilder.Append("LEFT JOIN GoodBasic B  ");
            strBuilder.Append("ON S.GoodID = B.ID  ");
            strBuilder.Append("WHERE S.GoodStatus = 'T'  ");
            strBuilder.Append("AND B.FLAG = 'T'   ");
            if (!string.IsNullOrEmpty(good))
            {
                strBuilder.Append("AND (B.GoodName LIKE '%"+ good + "%' OR PackBarcode = '"+ good + "' OR SubPackBarcode = '"+ good + "') ");
            }
            strBuilder.Append("group by s.GoodID,b.GoodName,  s.GoodUnit ");
            strBuilder.Append("ORDER BY GoodName, GoodUnit ");
            return strBuilder.ToString();
        }

        public static string OffShelf(int qty, string shelf, string id)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE GOODSHELF SET GoodStatus = 'F' WHERE ID IN  ");
            strBuilder.Append("(  ");
            strBuilder.Append("SELECT TOP "+ qty + " ID FROM GOODSHELF WHERE GoodStatus = 'T' AND SHELF = '"+ shelf + "' AND GOODID = '"+ id + "' ");
            strBuilder.Append(")  ");
            return strBuilder.ToString();
        }

        public static string GetSysUser(string id, string psd)
        {
            strBuilder  = new StringBuilder();
            strBuilder.Append("SELECT USERID, USERROLE, USERNAME, PSD FROM SYSUSER ");
            strBuilder.Append("WHERE USERNAME = '"+id+"'");
            return strBuilder.ToString();
        }

        public static string ChgPsd(string id, string psd)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("UPDATE SYSUSER SET PSD = '"+psd+"' WHERE USERNAME = '"+id+"'");
            return strBuilder.ToString();
        }
    }
}