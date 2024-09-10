﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Web;

namespace zym_api.Helper
{
    public class ImageHelper
    {
        public static byte[] ResizeImage(MemoryStream ms, int targetWidth)
        {
            Bitmap b = new Bitmap(ms);
            return DoResizeImage(b, targetWidth);
        }
        private static byte[] DoResizeImage(Bitmap b, int targetWidth)
        {
            Bitmap bn;
            int oHeight, oWidth, nHeight, nWidth;
            oHeight = b.Height;
            oWidth = b.Width;

            byte[] bt;

            if (oWidth <= targetWidth)
            {
                targetWidth = oWidth;
            }

            nHeight = oHeight * targetWidth / oWidth;
            nWidth = targetWidth;

            Rectangle rgO = new Rectangle(0, 0, oWidth, oHeight);
            Rectangle rgN = new Rectangle(0, 0, nWidth, nHeight);


            bn = new Bitmap(nWidth, nHeight);
            Graphics g = Graphics.FromImage(bn);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(b, rgN, rgO, GraphicsUnit.Pixel);
            g.Dispose();
            //Image image = b.GetThumbnailImage(oWidth, oHeight, null, System.IntPtr.Zero);//图像质量损失大

            MemoryStream ms2 = new MemoryStream();
            bn.Save(ms2, System.Drawing.Imaging.ImageFormat.Jpeg);
            bt = new byte[(int)ms2.Length];
            ms2.Position = 0;
            ms2.Read(bt, 0, (int)ms2.Length);
            ms2.Close();
            ms2.Dispose();
            ms2 = null;
            bn.Dispose();
            bn = null;
            g.Dispose();
            g = null;

            return bt;
        }
    }
}