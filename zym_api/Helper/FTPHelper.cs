using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Web;

namespace zym_api.Helper
{
    public class FTPHelper
    {
        public static string strFtpPath = @"ftp://49.233.191.59/";
        public static string strUser = @"49.233.191.59\zym";
        public static string strPwd = "!QAZxsw23edc";

        public static bool CheckFtp()
        {
            try
            {
                Log.WriteLog("Start Check FTP");
                FtpWebRequest ftpRequest = (FtpWebRequest)FtpWebRequest.Create(new Uri(strFtpPath));
                Log.WriteLog("Uri Finished.");
                ftpRequest.Credentials = new NetworkCredential(strUser, strPwd);
                Log.WriteLog("Credentials");
                ftpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                Log.WriteLog("Method");
                ftpRequest.Timeout = 3000;
                FtpWebResponse response = (FtpWebResponse)ftpRequest.GetResponse();
                Log.WriteLog("response");
                response.Close();
                return true;
            }
            catch (Exception ex)
            {
                Log.WriteLog(ex.ToString());
                throw new Exception(ex.Message);
                return false;
            }
        }

        public static void Upload(HttpPostedFile file, string strToPath, string strGuid)
        {
            //bool isExist =  DirectoryExists(strFtpPath + strToPath.Replace(@"\", @"/"));
            //FileInfo fileInf = new FileInfo(Path.GetFileName(file.FileName));
            //FtpWebRequest reqFTP;
            //reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(strFtpPath + strToPath.Replace(@"\",@"/") + "/" + fileInf.Name));
            //reqFTP.Credentials = new NetworkCredential(strUser, strPwd);
            //reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            //reqFTP.KeepAlive = false;
            //reqFTP.UseBinary = true;
            //reqFTP.ContentLength = fileInf.Length;
            //int buffLength = 2048;
            //byte[] buff = new byte[buffLength];
            //int contentLen;
            //FileStream fs = fileInf.OpenRead();
            //try
            //{
            //    Stream strm = reqFTP.GetRequestStream();
            //    contentLen = fs.Read(buff, 0, buffLength);
            //    while(contentLen != 0)
            //    {
            //        strm.Write(buff, 0, contentLen);
            //        contentLen = fs.Read(buff, 0, contentLen);
            //    }
            //    strm.Close();
            //    fs.Close();
            //}
            bool isExist = DirectoryExists(strFtpPath + strToPath.Replace(@"\", @"/"));
            FtpWebRequest reqFTP;
            Stream strm = null;

            try
            {
                Delete(strFtpPath + strToPath.Replace(@"\", @"/") + "/" + strGuid + ".jpg");
            }
            catch
            {

            }


            
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(strFtpPath + strToPath.Replace(@"\", @"/") + "/" + strGuid + ".jpg"));
            reqFTP.Credentials = new NetworkCredential(strUser, strPwd);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.KeepAlive = false;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = file.ContentLength;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            Stream fs = file.InputStream;
            try
            {
                strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, contentLen);
                }
                strm.Close();
                fs.Close();
            }
            catch (Exception ex)
            {

            }
        }

        public static void MakeDirectory(string directory)
        {
            var request = (FtpWebRequest)WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.MakeDirectory;
            request.Credentials = new NetworkCredential(strUser, strPwd);
            try
            {
                using (var resp = (FtpWebResponse)request.GetResponse()) // Exception occurs here
                {
                }
            }
            catch (WebException ex)
            {
            }
        }

        public static bool DirectoryExists(string directory)
        {
            bool directoryExists;
            var request = (FtpWebRequest)WebRequest.Create(directory);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            request.Credentials = new NetworkCredential(strUser, strPwd);
            try
            {
                using (request.GetResponse())
                {
                    directoryExists = true;
                }
            }
            catch (WebException)
            {
                directoryExists = false;
            }
            return directoryExists;
        }

        public static bool Upload(string strLocal, string strRemotePath, string strFtpID, string strFtpPwd)
        {
            bool isSuc = false;
            FileInfo fileInf = new FileInfo(strLocal);
            FtpWebRequest reqFTP;
            Stream strm = null;
            reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(strFtpPath + strRemotePath + "/" + fileInf.Name));
            reqFTP.Credentials = new NetworkCredential(strFtpID, strFtpPwd);
            reqFTP.Method = WebRequestMethods.Ftp.UploadFile;
            reqFTP.KeepAlive = false;
            reqFTP.UseBinary = true;
            reqFTP.ContentLength = fileInf.Length;
            int buffLength = 2048;
            byte[] buff = new byte[buffLength];
            int contentLen;
            Stream fs = fileInf.OpenRead();
            try
            {
                strm = reqFTP.GetRequestStream();
                contentLen = fs.Read(buff, 0, buffLength);
                while (contentLen != 0)
                {
                    strm.Write(buff, 0, contentLen);
                    contentLen = fs.Read(buff, 0, buffLength);
                }
                isSuc = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            finally
            {
                strm.Close();
                fs.Close();
            }
            return isSuc;
        }

        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="strRemotePath"></param>
        /// <param name="strFtpID"></param>
        /// <param name="strFtpPwd"></param>
        public static void Delete(string strRemotePat)
        {
            try
            {
                string uri = strRemotePat;
                FtpWebRequest reqFTP;
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(uri));

                reqFTP.Credentials = new NetworkCredential(strUser, strPwd);
                reqFTP.KeepAlive = false;
                reqFTP.Method = WebRequestMethods.Ftp.DeleteFile;

                string result = String.Empty;
                FtpWebResponse response = (FtpWebResponse)reqFTP.GetResponse();
                long size = response.ContentLength;
                Stream datastream = response.GetResponseStream();
                StreamReader sr = new StreamReader(datastream);
                result = sr.ReadToEnd();
                sr.Close();
                datastream.Close();
                response.Close();
                //Buffer.Log(string.Format("Ftp文件{1}?除成功！", DateTime.Now.ToString(), fileName));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取当前目录下文件列表(仅文件)
        /// </summary>
        /// <returns></returns>
        public static string[] GetFileList(string strPath, string mask = "*.*")
        {
            string[] downloadFiles = null;
            StringBuilder result = new StringBuilder();
            FtpWebRequest reqFTP;
            try
            {
                reqFTP = (FtpWebRequest)FtpWebRequest.Create(new Uri(strPath));
                reqFTP.UseBinary = true;
                reqFTP.Credentials = new NetworkCredential(strUser, strPwd);
                reqFTP.Method = WebRequestMethods.Ftp.ListDirectory;
                WebResponse response = reqFTP.GetResponse();
                StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);

                string line = reader.ReadLine();
                while (line != null)
                {
                    if (mask.Trim() != string.Empty && mask.Trim() != "*.*")
                    {

                        string mask_ = mask.Substring(0, mask.IndexOf("*"));
                        if (line.Substring(0, mask_.Length) == mask_)
                        {
                            result.Append(line);
                            result.Append("\n");
                        }
                    }
                    else
                    {
                        result.Append(line);
                        result.Append("\n");
                    }
                    line = reader.ReadLine();
                }
                if (result.Length == 0)
                    return new string[] { };
                result.Remove(result.ToString().LastIndexOf('\n'), 1);
                reader.Close();
                response.Close();
                downloadFiles = result.ToString().Split('\n');
            }
            catch (Exception ex)
            {
                return new string[] { };
            }

            if (downloadFiles == null)
                downloadFiles = new string[] { };
            return downloadFiles;
        }

        public static void RemoveDictionary(string strPath)
        {
            try
            {
                // 创建 FtpWebRequest 对象
                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(FTPHelper.strFtpPath + strPath);
                request.Method = WebRequestMethods.Ftp.RemoveDirectory;

                // 设置 FTP 认证信息
                request.Credentials = new NetworkCredential(strUser, strPwd);

                // 发送请求并获取响应
                FtpWebResponse response = (FtpWebResponse)request.GetResponse();
                Console.WriteLine("删除成功");
                response.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}