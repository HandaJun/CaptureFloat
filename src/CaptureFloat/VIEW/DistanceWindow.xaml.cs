using CaptureFloat.LOGIC;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Rectangle = System.Drawing.Rectangle;

namespace CaptureFloat.VIEW
{
    /// <summary>
    /// Interaction logic for CaptureHoldWindow.xaml
    /// </summary>
    public partial class DistanceWindow : Window
    {
        double RectTop = 0;
        double RectLeft = 0;
        double MouseX = 0;
        double MouseY = 0;
        bool IsRect = false;
        string distanceStr = null;
        readonly ScreenCapturer sc = new ScreenCapturer();
        Bitmap bitmap = null;
        readonly ScreenManager screenManager = null;
        Rect rect = new Rect();

        public DistanceWindow(ScreenManager sm)
        {
            InitializeComponent();
            screenManager = sm;
            rect = screenManager.DeviceBounds;
            Left = rect.Left;
            Top = rect.Top;
            Width = rect.Width;
            Height = rect.Height;

            SetCapture();
            StartRect.Visibility = Visibility.Collapsed;
            EndRect.Visibility = Visibility.Collapsed;
            DistanceTb.Visibility = Visibility.Collapsed;
            VerticalRect.Height = rect.Height;
            HorizontalRect.Width = rect.Width;
        }

        public void Open()
        {
            this.Show();
        }

        private void RePaint()
        {
            double ScreenHeight = overGd.ActualHeight;
            double ScreenWidth = overGd.ActualWidth;

            EndRect.Margin = new Thickness(MouseX - 3, MouseY - 3, ScreenWidth - MouseX - 3, ScreenHeight - MouseY - 3);

            double disX = MouseX - RectLeft;
            double disY = MouseY - RectTop;
            distanceStr = $"{(int)disX}, {(int)disY}";
            DistanceTb.Text = $" {distanceStr} ";
            DistanceTb.Margin = new Thickness(MouseX + 5, MouseY + 5, 0, 0);
        }

        private void DashRePaint()
        {
            VerticalRect.Margin = new Thickness(MouseX - 0.5, 0, rect.Width - MouseX - 0.5, 0);
            HorizontalRect.Margin = new Thickness(0, MouseY - 0.5, 0, rect.Height - MouseY - 0.5);
        }

        private void OverGd_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(overGd);
            RectLeft = mousePos.X;
            RectTop = mousePos.Y;
            IsRect = true;
            StartRect.Visibility = Visibility.Visible;

            double ScreenHeight = overGd.ActualHeight;
            double ScreenWidth = overGd.ActualWidth;
            StartRect.Margin = new Thickness(mousePos.X - 3, mousePos.Y - 3, ScreenWidth - mousePos.X - 3, ScreenHeight - mousePos.Y - 3);

            EndRect.Visibility = Visibility.Visible;
            DistanceTb.Visibility = Visibility.Visible;

            RePaint();
        }

        private void OverGd_MouseUp(object sender, MouseButtonEventArgs e)
        {
            IsRect = false;
            var mousePos = e.GetPosition(overGd);
            MouseX = mousePos.X;
            MouseY = mousePos.Y;
            RePaint();
            End();
            //Capture();
            this.Close();
        }

        public void End()
        {
            foreach (var dis in MainWindow.disWindows)
            {
                dis.Close();
            }
            MainWindow.GetInstance().Visibility = Visibility.Visible;
            Clipboard.Clear();
            Clipboard.SetText(distanceStr);
            MessageBox.Show(distanceStr + "\n[クリップボードにコピーしました。]");
            MainWindow.GetInstance().FloatWindowTopMost(true);
        }

        private void OverGd_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(overGd);
            MouseX = mousePos.X;
            MouseY = mousePos.Y;

            DashRePaint();

            if (IsRect)
            {
                RePaint();
            }
        }

        public void SetImage(BitmapImage img)
        {
            BackgroundImg.Source = img;
        }

        private void SetCapture()
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
            BackgroundImg.Source = BitmapToImage(bitmap);
        }

        public BitmapImage BitmapToImage(Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                return bitmapImage;
            }
        }

        public Bitmap CropAtRect(Bitmap b, Rectangle r)
        {
            Bitmap nb = new Bitmap(r.Width, r.Height);
            using (Graphics g = Graphics.FromImage(nb))
            {
                g.DrawImage(b, -r.X, -r.Y);
                return nb;
            }
        }

        private void OverGd_MouseEnter(object sender, MouseEventArgs e)
        {
            VerticalRect.Visibility = Visibility.Visible;
            HorizontalRect.Visibility = Visibility.Visible;
        }

        private void OverGd_MouseLeave(object sender, MouseEventArgs e)
        {
            VerticalRect.Visibility = Visibility.Collapsed;
            HorizontalRect.Visibility = Visibility.Collapsed;
        }
    }
}
