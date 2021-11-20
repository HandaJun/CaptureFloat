using CaptureFloat.LOGIC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using Rectangle = System.Drawing.Rectangle;

namespace CaptureFloat.VIEW
{
    public partial class CaptureHoldWindow : Window
    {
        double RectTop = 0;
        double RectLeft = 0;
        double MouseX = 0;
        double MouseY = 0;
        double RectWidth = 0;
        double RectHeight = 0;
        bool IsRect = false;

        readonly int ZoomRectSize = 200;
        readonly ScreenCapturer sc = new ScreenCapturer();
        Bitmap bitmap = null;
        readonly ScreenManager screenManager = null;
        Rect rect = new Rect();
        public double ScreenScale = 0;
        public double Magnification = 1;

        public int NowMoveKey = 0;

        public CaptureHoldWindow(ScreenManager sm)
        {
            InitializeComponent();
            screenManager = sm;
            rect = screenManager.DeviceBounds;
            Left = rect.Left * MainWindow.Magnification;
            Top = rect.Top * MainWindow.Magnification;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            ScreenScale = ScreenManager.GetScale(this);

            if (ScreenScale == 1.25d)
            {
                Magnification = (4d / 5d);
            }
            else if (ScreenScale == 1.50d)
            {
                Magnification = (2d / 3d);
            }

            Width = rect.Width * Magnification;
            Height = rect.Height * Magnification;

            SetCapture();
            SelectedRect.Visibility = Visibility.Collapsed;
            VerticalRect.Height = rect.Height * Magnification;
            HorizontalRect.Width = rect.Width * Magnification;

            BackgroundImg.RenderSize = new System.Windows.Size(bitmap.Width * Magnification, bitmap.Height * Magnification);

            ZoomBd.Width = ZoomBd.Height = ZoomRectSize + 2;
            Topmost = false;
            Topmost = true;
            Activate();

        }

        public void Open()
        {
            this.ShowInTaskbar = false;
            this.Show();
            NativeMethods.SetWindowPos(new WindowInteropHelper(this).Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE);
        }

        private void RePaint()
        {
            SelectedRect.Visibility = Visibility.Visible;

            RectWidth = Math.Abs(RectLeft - MouseX) + 1;
            RectHeight = Math.Abs(RectTop - MouseY) + 1;

            double ScreenHeight = overGd.ActualHeight;
            double ScreenWidth = overGd.ActualWidth;

            double left;
            double top;
            double right;
            double bottom;

            if (RectTop < MouseY)
            {
                top = RectTop;
                bottom = ScreenHeight - (RectTop + RectHeight);
            }
            else
            {
                top = MouseY;
                bottom = ScreenHeight - (MouseY + RectHeight);
            }

            if (RectLeft < MouseX)
            {
                left = RectLeft;
                right = ScreenWidth - (RectLeft + RectWidth);
            }
            else
            {
                left = MouseX;
                right = ScreenWidth - (MouseX + RectWidth);
            }

            SelectedRect.Width = RectWidth;
            SelectedRect.Height = RectHeight;
            SelectedRect.Margin = new Thickness(left, top, right, bottom);
        }

        private void DashRePaint()
        {
            VerticalRect.Margin = new Thickness(MouseX, 0, rect.Width * Magnification - MouseX - 1, 0);
            HorizontalRect.Margin = new Thickness(0, MouseY, 0, rect.Height * Magnification - MouseY - 1);
        }

        private void OverGd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(overGd);
            RectTop = mousePos.Y;
            RectLeft = mousePos.X;
            RectWidth = 0;
            RectHeight = 0;
            IsRect = true;
            RePaint();
        }

        private void OverGd_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsRect = false;
            var mousePos = e.GetPosition(overGd);
            MouseX = mousePos.X;
            MouseY = mousePos.Y;
            RePaint();
            Capture();
            this.Close();
        }

        private void Capture()
        {
            double left;
            double top;

            if (RectTop < MouseY)
            {
                top = RectTop;
            }
            else
            {
                top = MouseY;
            }

            if (RectLeft < MouseX)
            {
                left = RectLeft;
            }
            else
            {
                left = MouseX;
            }

            if (RectWidth != 0 && RectHeight != 0)
            {
                Rect rect = new Rect(left, top, RectWidth, RectHeight);
                var crop = CropAtRect(bitmap, rect);
                crop.Save(Common.ImgFolder + "\\" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".png");

                if (App.Setting.IsOnlyClipboard)
                {
                    Clipboard.Clear();
                    try
                    {
                        Clipboard.SetImage(BitmapToImage(crop));
                    }
                    catch
                    {
                    }
                }
                else
                {
                    new FloatWindow().Open(BitmapToImage(crop), rect, screenManager.DeviceBounds);
                }
            }

            foreach (var hold in MainWindow.holdWindows)
            {
                hold.Close();
            }
            Common.IsCapture = false;
            Common.SetAllWindowState(WindowState.Normal, "FloatWindow", "MainWindow");
        }

        private void OverGd_MouseMove(object sender, MouseEventArgs e)
        {
            if (NowMoveKey > 10000)
            {
                NowMoveKey = 0;
            }
            NowMoveKey++;
            var mousePos = e.GetPosition(overGd);
            MouseX = mousePos.X;
            MouseY = mousePos.Y;

            DashRePaint();

            if (IsRect)
            {
                RePaint();
            }
            ZoomPaint(e, NowMoveKey);
        }

        public void SetImage(BitmapImage img)
        {
            BackgroundImg.Source = img;
        }

        private void SetCapture()
        {
            if (bitmap != null)
            {
                BackgroundImg.Source = BitmapToImage(bitmap);
            }
            {
                var bounds = screenManager.DeviceBounds;
                ScreenCapturer.Rect rect = new ScreenCapturer.Rect()
                {
                    Top = (int)bounds.Top,
                    Left = (int)bounds.Left,
                    Bottom = (int)bounds.Bottom,
                    Right = (int)bounds.Right,
                };
                bitmap = sc.Capture(rect, enmScreenCaptureMode.Screen);
                BitmapImage bi = BitmapToImage(bitmap);
                BackgroundImg.Source = bi;
            }
        }

        public void SetBackground(Bitmap bit)
        {
            bitmap = bit;
        }

        public BitmapImage BitmapToImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                RenderOptions.SetBitmapScalingMode(bitmapImage, BitmapScalingMode.NearestNeighbor);
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public Bitmap ResizeImage(
                        Bitmap image,
                        int width,
                        int height)
        {
            var destRect = new System.Drawing.Rectangle(
              0, 0, width, height);

            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution,
              image.VerticalResolution);

            using (var g = Graphics.FromImage(destImage))
            {
                g.CompositingMode = CompositingMode.SourceCopy;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    g.DrawImage(image, destRect, 0, 0, image.Width,
                      image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }
            return destImage;
        }

        public Bitmap CropAtRect(Bitmap b, Rect r)
        {
            Bitmap nb = new Bitmap((int)(r.Width * ScreenScale), (int)(r.Height * ScreenScale));
            using (Graphics g = Graphics.FromImage(nb))
            {
                g.DrawImage(b, (int)(-r.X * ScreenScale), (int)(-r.Y * ScreenScale));
                return nb;
            }
        }

        private void OverGd_MouseEnter(object sender, MouseEventArgs e)
        {
            VerticalRect.Visibility = Visibility.Visible;
            HorizontalRect.Visibility = Visibility.Visible;
            Focus();
            ZoomBd.Visibility = Visibility.Visible;
        }

        private void OverGd_MouseLeave(object sender, MouseEventArgs e)
        {
            VerticalRect.Visibility = Visibility.Collapsed;
            HorizontalRect.Visibility = Visibility.Collapsed;
            ZoomBd.Visibility = Visibility.Collapsed;
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                foreach (var hold in MainWindow.holdWindows)
                {
                    hold.Close();
                }
                Common.SetAllWindowState(WindowState.Normal, "FloatWindow", "MainWindow");
                Common.IsCapture = false;
            }
        }

        public int ZoomDistance = 50;

        public void ZoomPaint(MouseEventArgs e, int key)
        {
            double ScreenHeight = overGd.ActualHeight;
            double ScreenWidth = overGd.ActualWidth;
            bool IsZoomLeft = false;
            bool IsZoomTop = false;

            var mousePos = e.GetPosition(overGd);

            if (ScreenWidth - ZoomRectSize - ZoomDistance > mousePos.X)
            {
                IsZoomLeft = true;
            }
            if (ScreenHeight - ZoomRectSize - ZoomDistance > mousePos.Y)
            {
                IsZoomTop = true;
            }

            if (NowMoveKey != key) return;

            ScreenCapturer.Rect captureRect = new ScreenCapturer.Rect()
            {
                Top = (int)mousePos.Y - 20 + (int)rect.Top,
                Left = (int)mousePos.X - 20 + (int)rect.Left,
                Bottom = (int)mousePos.Y + 20 + (int)rect.Top,
                Right = (int)mousePos.X + 20 + (int)rect.Left,
            };

            if (true)
            {
                var crop = sc.Capture(captureRect, enmScreenCaptureMode.Screen);
                if (NowMoveKey != key) return;
                SetZoomBd(crop);
            }

            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(50);
                var crop = sc.Capture(captureRect, enmScreenCaptureMode.Screen);
                if (NowMoveKey != key) return;
                ZoomBd.Dispatcher.Invoke(DispatcherPriority.Normal, (Action)(() =>
                {
                    SetZoomBd(crop);
                }));
            });

            void SetZoomBd(Bitmap crop)
            {
                ZoomImg.Source = BitmapToImage(crop);
                if (NowMoveKey != key) return;

                if (IsZoomLeft && IsZoomTop)
                {
                    ZoomBd.Margin = new Thickness(mousePos.X + ZoomDistance, mousePos.Y + ZoomDistance, 0, 0);
                }
                else if (!IsZoomLeft && IsZoomTop)
                {
                    ZoomBd.Margin = new Thickness(mousePos.X - ZoomRectSize - ZoomDistance, mousePos.Y + ZoomDistance, 0, 0);
                }
                else if (IsZoomLeft && !IsZoomTop)
                {
                    ZoomBd.Margin = new Thickness(mousePos.X + ZoomDistance, mousePos.Y - ZoomRectSize - ZoomDistance, 0, 0);
                }
                else if (!IsZoomLeft && !IsZoomTop)
                {
                    ZoomBd.Margin = new Thickness(mousePos.X - ZoomRectSize - ZoomDistance, mousePos.Y - ZoomRectSize - ZoomDistance, 0, 0);
                }
            }

        }
    }
}
