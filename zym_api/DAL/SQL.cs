using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
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
            strBuilder.Append("'" + entity.SubPackUnit + "',");
            strBuilder.Append("'" + entity.SubPackBarcode + "',");
            strBuilder.Append("'" + entity.SubPackQty + "',");
            strBuilder.Append("'" + entity.Picture + "'");
            strBuilder.Append(")");
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
            strBuilder.Append("C.Category, B.ID, B.GoodName, B.PackUnit, B.PackBarcode, B.IsHasSubPack, B.SubPackUnit, B.SubPackBarcode, B.SubPackQty, B.Picture ");
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
                strBuilder.Append("AND B.GOODNAME = '"+goodName+"' ");
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
            strBuilder.Append("SELECT ID, GoodName, PackUnit, 'OUT' AS PACK FROM GOODBASIC where PackBarcode= '" + barcode + "' ");
            strBuilder.Append("UNION ALL ");
            strBuilder.Append("SELECT ID, GoodName, SubPackUnit, 'IN' AS PACK FROM GOODBASIC where SubPackBarcode= '" + barcode + "' ");
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
            strBuilder.Append("'" + shelf + "',");
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

        public static string GetBasicOutByBarcode(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, PACKUNIT as Unit FROM GoodBasic WHERE  FLAG = 'T' AND PackBarcode = '"+barcode+"'");
            return strBuilder.ToString();
        }

        public static string GetBasicInByBarcode(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, SUBPACKUNIT  as Unit FROM GoodBasic WHERE  FLAG = 'T' AND SubPackBarcode = '" + barcode + "'");
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
            strBuilder.Append("select b.GoodName, sum(s.goodqty) as qty, s.GoodUnit, s.Shelf ");
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
            strBuilder.Append("group by b.GoodName, s.GoodUnit, s.Shelf ");
            strBuilder.Append("ORDER BY GoodName, GoodUnit, SHELF ");
            return strBuilder.ToString();
        }

        public static string ReqGood(string query)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT GoodID, GoodName, sum(goodqty) as Qty, GoodUnit AS Unit FROM GoodShelf S  ");
            strBuilder.Append("LEFT JOIN GoodBasic B  ");
            strBuilder.Append("ON S.GoodID = B.ID  ");
            strBuilder.Append("WHERE S.GoodStatus = 'T'  ");
            strBuilder.Append("AND B.FLAG = 'T'  ");
            strBuilder.Append("AND (B.GoodName LIKE '%"+ query + "%' OR PackBarcode = '"+ query + "' OR SubPackBarcode = '"+ query + "')  ");
            strBuilder.Append("group by GoodID, b.GoodName, s.GoodUnit  ");
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
            strBuilder.Append("SELECT isnull(sum(GoodQty), '0'), 'TOTAL' ");
            strBuilder.Append("FROM GOODSHELF ");
            strBuilder.Append("WHERE GoodStatus = 'T' ");
            strBuilder.Append("AND GoodID = '" + goodID + "' ");
            strBuilder.Append("AND GoodUnit = '"+ unit + "'");
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
            strBuilder.Append("SELECT * FROM PAGELIST ORDER BY PAGEGROUP, PAGENAME");
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
            strBuilder.Append("AND GoodUnit = '"+ unit + "' ");
            strBuilder.Append("AND GoodID = '"+goodID+"' ");
            strBuilder.Append("ORDER BY SHELF ");
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
            strBuilder.Append("SELECT ID, PackUnit AS UNIT, 'OUT' AS PACK FROM GoodBasic ");
            strBuilder.Append("WHERE FLAG = 'T' ");
            strBuilder.Append("AND PackBarcode = '"+ barcode + "' ");
            return strBuilder.ToString();
        }

        public static string GetInPack(string barcode)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT ID, SUBPACKUNIT AS UNIT, 'OUT' AS PACK FROM GoodBasic ");
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
            strBuilder.Append("and GoodUnit = N'"+unit+"' ");
            return strBuilder.ToString();
        }

        public static string GetReqByID(string goodID, string unit, string reqNo)
        {
            strBuilder = new StringBuilder();
            strBuilder.Append("SELECT QUANTITY FROM GoodRequest ");
            strBuilder.Append("WHERE RequestStatus = 'T' ");
            strBuilder.Append("AND ID = '"+reqNo+"' ");
            strBuilder.Append("AND GOODID = '"+goodID+"' ");
            strBuilder.Append("AND GoodUnit = N'"+unit+"' ");
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
            strBuilder.Append("update GoodRequest set RequestStatus = 'F' where id = '"+ reqNo + "' and goodid = '"+ goodID + "' and GoodUnit = '"+ unit + "' ");
            return strBuilder.ToString();
        }
    }
}