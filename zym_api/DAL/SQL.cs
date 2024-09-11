using System;
using System.Collections.Generic;
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
            return strBuilder.ToString();
        }
    }
}