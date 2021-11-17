using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaptureFloat
{
    class ScreenCapture
    {
        //private Bitmap FImage;
        //private Bitmap PImage;
        //private Point p;
        //private int width = 0;
        //private int height = 0;
        //private void PrintDoc_PrintPage(object sender, PrintPageEventArgs e)
        //{
        //    OverPanel.AutoSize = true;
        //    int x = 0;
        //    int y = 0;
        //    int width = OverPanel.Width;
        //    int height = OverPanel.Height;

        //    Rectangle bounds = new Rectangle(x, y, width, height);

        //    PImage = new Bitmap(width, height);

        //    OverPanel.DrawToBitmap(PImage, bounds);

        //    p = new Point(0, 0);
        //    label1.Text = width.ToString() + " " + height.ToString();
        //    int bound1 = 0;
        //    int bound2 = 0;
        //    float boundsH = e.Graphics.VisibleClipBounds.Height;
        //    float boundsW = e.Graphics.VisibleClipBounds.Width;
        //    float boundsS = e.PageBounds.Height;
        //    float boundsE = e.PageBounds.Width;
        //    float CBound1 = boundsS - boundsH;
        //    float CBound2 = boundsE - boundsW;
        //    float boundHD = (boundsH - CBound1);
        //    float boundHW = (boundsW - CBound2);
        //    int bounds1 = Convert.ToInt32(boundHD);
        //    int bounds2 = Convert.ToInt32(boundHW);
        //    int check1 = ((bounds1 * 100) / OverPanel.Height);
        //    int check2 = ((bounds2 * 100) / OverPanel.Width);
        //    if (check1 < check2)
        //    {
        //        bound1 = (OverPanel.Height * check1) / 100;
        //        bound2 = (OverPanel.Width * check1) / 100;
        //    }
        //    else
        //    {
        //        bound1 = (OverPanel.Height * check2) / 100;
        //        bound2 = (OverPanel.Width * check2) / 100;
        //    }
        //    Point TPoint = new Point(0, 0);
        //    Graphics HQImage = Graphics.FromImage(PImage);
        //    HQImage.CompositingQuality = CompositingQuality.HighQuality;
        //    HQImage.InterpolationMode = InterpolationMode.HighQualityBicubic;
        //    HQImage.SmoothingMode = SmoothingMode.HighQuality;

        //    FImage = new Bitmap(PImage, bound2, bound1);

        //    HQImage.DrawImage(FImage, TPoint);

        //    e.Graphics.DrawImage(FImage, p);
        //}

        //private void button1_Click_1(object sender, EventArgs e)
        //{
        //    PrintDocument PrintDoc = new PrintDocument();
        //            PrintDoc.PrintPage += PrintDoc_PrintPage;
        //    PrintDoc.Print();
        //    FImage.Save("C:\\Test\\TestBMP.bmp");
        //    PImage.Save("C:\\Test\\OBMP.bmp");

        //}

        //private static void TakeScreenshotPrivate(string strFilename)
        //{
        //    Rectangle objRectangle = Screen.PrimaryScreen.Bounds;
        //    Bitmap objBitmap = new Bitmap(objRectangle.Right, objRectangle.Bottom);
        //    Graphics objGraphics = default(Graphics);
        //    IntPtr hdcDest = default(IntPtr);
        //    int hdcSrc = 0;
        //    const int SRCCOPY = 0xcc0020;
        //    string strFormatExtension = null;

        //    objGraphics = Graphics.FromImage(objBitmap);

        //    //-- get a device context to the windows desktop and our destination  bitmaps
        //    hdcSrc = GetDC(0);
        //    hdcDest = objGraphics.GetHdc;
        //    //-- copy what is on the desktop to the bitmap
        //    BitBlt(hdcDest.ToInt32, 0, 0, objRectangle.Right, objRectangle.Bottom, hdcSrc, 0, 0, SRCCOPY);
        //    //-- release device contexts
        //    objGraphics.ReleaseHdc(hdcDest);
        //    ReleaseDC(0, hdcSrc);

        //    strFormatExtension = _ScreenshotImageFormat.ToString.ToLower;
        //    if (System.IO.Path.GetExtension(strFilename) != "." + strFormatExtension)
        //    {
        //        strFilename += "." + strFormatExtension;
        //    }
        //    switch (strFormatExtension)
        //    {
        //        case "jpeg":
        //            BitmapToJPEG(objBitmap, strFilename, 80);
        //            break;
        //        default:
        //            objBitmap.Save(strFilename, _ScreenshotImageFormat);
        //            break;
        //    }

        //    //-- save the complete path/filename of the screenshot for possible later use
        //    _strScreenshotFullPath = strFilename;
        //}
    }
}
