using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
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
    }
}