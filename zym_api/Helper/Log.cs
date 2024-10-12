using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services.Description;

namespace zym_api.Helper
{
    public class Log
    {
        public static void WriteLog(string log)
        {
            string strPath = @"C:\Log\" + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            StringBuilder str = new StringBuilder();
            str.Append(log);
            StreamWriter sw;
            if (!File.Exists(strPath))
            {
                sw = File.CreateText(strPath);
            }
            else
            {
                sw = File.AppendText(strPath);
            }

            sw.WriteLine(str.ToString());
            sw.Close();


        }
    }
}