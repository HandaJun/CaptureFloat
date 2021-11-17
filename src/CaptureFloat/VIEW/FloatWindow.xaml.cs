using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using CaptureFloat.LOGIC;
using Microsoft.Win32;

namespace CaptureFloat.VIEW
{
    /// <summary>
    /// Interaction logic for FloatWindow.xaml
    /// </summary>
    public partial class FloatWindow : Window
    {
        bool PenMode = false;
        BitmapSource nowImg = null;
        BitmapSource canImg = null;
        double ratio = 1;
        bool FaintFlg = false;
        double tempScale = 1;
        double Magnification = 1;
        bool PinFlg = true;

        public FloatWindow()
        {
            InitializeComponent();
        }

        public void SetImage(BitmapSource bitmap)
        {
            MainImg.Source = bitmap;
            nowImg = bitmap;
            ratio = nowImg.Width / nowImg.Height;
            Clipboard.Clear();
            try
            {
                Clipboard.SetImage(bitmap);
            }
            catch
            {
            }
        }

        public void Open(BitmapSource bitmap, Rect rect, Rect deviceBounds)
        {
            Width = 1;
            Height = 1;
            Left = deviceBounds.X + rect.X;
            Top = deviceBounds.Y + rect.Y;
            this.Show();
            tempScale = ScreenManager.GetScale(this);
            this.Hide();

            Title = DateTime.Now.ToString("yyyyMMddHHmmss");

            if (MainWindow.ScreenScale < tempScale)
            {
                if (tempScale == 1.25d)
                {
                    Magnification = (4d / 5d);
                }
                else if (tempScale == 1.50d)
                {
                    Magnification = (2d / 3d);
                }
                Left = deviceBounds.X * Magnification + (double)rect.X - 10d - 1d * Magnification;
                Top = deviceBounds.Y * Magnification + (double)rect.Y - 10d - 1d * Magnification;
                Width = (double)bitmap.Width * Magnification + 20d + 2d * Magnification;
                Height = (double)bitmap.Height * Magnification + 20d + 2d * Magnification;
            }
            else if (MainWindow.ScreenScale > tempScale)
            {
                if (MainWindow.ScreenScale == 1.25d)
                {
                    Magnification = (5d / 4d);
                }
                else if (MainWindow.ScreenScale == 1.50d)
                {
                    Magnification = (3d / 2d);
                }
                Left = deviceBounds.X * tempScale + (double)rect.X - 10d - 1d * tempScale;
                Top = deviceBounds.Y * tempScale + (double)rect.Y - 10d - 1d * tempScale;
                Width = (double)bitmap.Width * Magnification + 20d + 2d * tempScale;
                Height = (double)bitmap.Height * Magnification + 20d + 2d * tempScale;
            }
            else
            {
                if (tempScale == 1.25d)
                {
                    Magnification = (4d / 5d);
                }
                else if (tempScale == 1.50d)
                {
                    Magnification = (2d / 3d);
                }

                Left = deviceBounds.X * Magnification + (double)rect.X - 10d - (1d * Magnification * MainWindow.Magnification);
                Top = deviceBounds.Y * Magnification + (double)rect.Y - 10d - (1d * Magnification * MainWindow.Magnification);
                Width = (double)bitmap.Width + 20d + 2d * Magnification;
                Height = (double)bitmap.Height + 20d + 2d * Magnification;
            }

            SetImage(bitmap);
            this.Show();
            MainWindow.floatWindows.Add(Title, this);
            SetTopMost();
        }

        public void SetTopMost(bool flg = true)
        {
            if (flg)
            {
                NativeMethods.SetWindowPos(new WindowInteropHelper(this).Handle, NativeMethods.HWND_TOPMOST, 0, 0, 0, 0, (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE);
            }
            else
            {
                NativeMethods.SetWindowPos(new WindowInteropHelper(this).Handle, NativeMethods.HWND_NOTOPMOST, 0, 0, 0, 0, (int)SetWindowPosFlags.SWP_NOACTIVATE | (int)SetWindowPosFlags.SWP_NOMOVE | (int)SetWindowPosFlags.SWP_NOSIZE);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                try
                {
                    this.DragMove();
                }
                catch (Exception)
                {
                }
            }
        }

        private void Window_MouseEnter(object sender, MouseEventArgs e)
        {
            CloseBt.Visibility = Visibility.Visible;
            PenBt.Visibility = Visibility.Visible;
            PinBt.Visibility = Visibility.Visible;
            //SaveBt.Visibility = Visibility.Visible;
        }

        private void Window_MouseLeave(object sender, MouseEventArgs e)
        {
            CloseBt.Visibility = Visibility.Hidden;
            PenBt.Visibility = Visibility.Hidden;
            PinBt.Visibility = Visibility.Hidden;
            //SaveBt.Visibility = Visibility.Hidden;
        }

        private void CloseBt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MainWindow.floatWindows.Remove(Title);
            this.Close();
        }

        private Cursor _cursor;

        private void OnResizeThumbDragStarted(object sender, DragStartedEventArgs e)
        {
            _cursor = Cursor;
            Cursor = Cursors.SizeNWSE;
        }

        private void OnResizeThumbDragCompleted(object sender, DragCompletedEventArgs e)
        {
            Cursor = _cursor;
        }

        private void OnResizeThumbDragDelta(object sender, DragDeltaEventArgs e)
        {
            //if(e.VerticalChange != 0)
            //{
            //    //OnResizeHeight(e);
            //}
            //else if(e.HorizontalChange != 0)
            //{
            //    OnResizeWidth(e);
            //}
            OnResizeWidth(e);
        }

        private void OnResizeThumbDragDeltaB(object sender, DragDeltaEventArgs e)
        {
            OnResizeHeight(e);
        }
        private void OnResizeThumbDragDeltaR(object sender, DragDeltaEventArgs e)
        {
            OnResizeWidth(e);
        }

        public void OnResizeHeight(DragDeltaEventArgs e)
        {
            try
            {
                double yAdjust = this.Height + e.VerticalChange;
                yAdjust = (this.ActualHeight + yAdjust) > this.MinHeight ? yAdjust : this.MinHeight;
                //if (yAdjust > this.MaxHeight)
                //{
                //    yAdjust = this.MaxHeight;
                //}
                this.Height = yAdjust;
                this.Width = ((yAdjust - 22) * ratio) + 22;
            }
            catch (Exception)
            {
            }
        }

        public void OnResizeWidth(DragDeltaEventArgs e)
        {
            try
            {
                double xAdjust = this.Width + e.HorizontalChange;
                xAdjust = (this.ActualWidth + xAdjust) > this.MinWidth ? xAdjust : this.MinWidth;
                //if (xAdjust > this.MaxWidth)
                //{
                //    xAdjust = this.MaxWidth;
                //}
                this.Width = xAdjust;
                this.Height = ((xAdjust - 22) / ratio) + 22;
            }
            catch (Exception)
            {
            }
        }

        private void OnCtrlC(object sender, ExecutedRoutedEventArgs e)
        {
            ImageToClipboard();
        }

        public void ImageToClipboard()
        {
            BitmapSource img = ImageMerge(new List<BitmapSource>() { nowImg, canImg });
            Clipboard.Clear();
            Clipboard.SetImage(img);
        }

        public BitmapSource ImageMerge(List<BitmapSource> list)
        {
            if (list.Count > 0)
            {
                Rect bounds = new Rect(0.0, 0.0, MainImg.ActualWidth, MainImg.ActualHeight);
                DrawingVisual visual = new DrawingVisual();
                DrawingContext dc = visual.RenderOpen();
                foreach (var item in list)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    dc.DrawImage(item, new Rect(bounds.X, bounds.Y, bounds.Width, bounds.Height));
                }
                dc.Close();
                RenderTargetBitmap bmp = new RenderTargetBitmap((int)bounds.Width, (int)bounds.Height, 96, 96, PixelFormats.Pbgra32);
                bmp.Render(visual);
                return bmp;
            }
            return null;
        }

        private void OnCtrlV(object sender, ExecutedRoutedEventArgs e)
        {
            ImagePaste();
        }

        public void ImagePaste()
        {
            if (Clipboard.ContainsImage())
            {
                string path = SaveImage(Clipboard.GetImage());
                NewOpen(path);
            }
        }

        public void NewOpen(string path)
        {
            var img = GetImage(path);
            double x = Left + (Width / 2) - (img.Width / 2);
            double y = Top + (Height / 2) - (img.Height / 2);
            Rect rect = new Rect(x, y, img.Width, img.Height);
            new FloatWindow().Open(img, rect, new Rect(0, 0, 0, 0));
            MainWindow.floatWindows.Remove(Title);
            this.Close();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string SaveImage(BitmapSource bit)
        {
            string tempPath = "";
            try
            {
                if (!Directory.Exists("Temp"))
                {
                    Directory.CreateDirectory("Temp");
                }
                //tempPath = @"Temp\" + Guid.NewGuid().ToString() + ".jpg";
                tempPath = @"Temp\" + Guid.NewGuid().ToString() + ".png";

                using (var fileStream = new FileStream(tempPath, FileMode.Create))
                {
                    //BitmapEncoder encoder = new JpegBitmapEncoder();
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(bit));
                    encoder.Save(fileStream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
            return tempPath;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BitmapSource GetImage(string path)
        {
            BitmapImage bmp = null;
            try
            {
                if (!Directory.Exists("Temp"))
                {
                    Directory.CreateDirectory("Temp");
                }
                string tempPath = @"Temp\" + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(path);
                File.Copy(path, tempPath);
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(tempPath, UriKind.Relative);
                RenderOptions.SetBitmapScalingMode(bmp, BitmapScalingMode.NearestNeighbor);
                bmp.EndInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
            return bmp;
        }

        private void PenBt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PenMode = !PenMode;
            if (PenMode)
            {
                PenCanvas.IsEnabled = true;
                PenCanvas.Width = MainImg.ActualWidth;
                PenCanvas.Height = MainImg.ActualHeight;
                PenTb.Text = "❐";
            }
            else
            {
                PenCanvas.IsEnabled = false;
                BitmapSource penImg = GetCanvasImage(PenCanvas);
                canImg = ImageMerge(new List<BitmapSource>() { canImg, penImg });
                if (canImg != null)
                {
                    CanvasImg.Source = canImg;
                }
                PenCanvas.Strokes.Clear();
                PenTb.Text = "✒";
            }
            //CanvasImg.Visibility = PenMode ? Visibility.Hidden : Visibility.Visible;
            //PenCanvas.Visibility = PenMode ? Visibility.Visible : Visibility.Collapsed;
            //PenCanvas.Width = CanvasImg.Width;
            //PenCanvas.Height = CanvasImg.Height;
        }

        public BitmapImage GetCanvasImage(InkCanvas surface)
        {
            try
            {
                double
                    x1 = surface.Margin.Left,
                    x2 = surface.Margin.Top,
                    x3 = surface.Margin.Right,
                    x4 = surface.Margin.Bottom;

                //if (path == null) return;
                surface.Margin = new Thickness(0, 0, 0, 0);
                Size size = new Size(surface.RenderSize.Width, surface.RenderSize.Height);
                surface.Measure(size);
                surface.Arrange(new Rect(size));

                RenderTargetBitmap renderBitmap =
                 new RenderTargetBitmap(
                   (int)size.Width,
                   (int)size.Height,
                   96,
                   96,
                   PixelFormats.Pbgra32);
                RenderOptions.SetBitmapScalingMode(renderBitmap, BitmapScalingMode.NearestNeighbor);
                renderBitmap.Render(surface);


                //using (FileStream fs = File.Open(path, FileMode.Create))
                //{
                //    PngBitmapEncoder encoder = new PngBitmapEncoder();
                //    encoder.Frames.Add(BitmapFrame.Create(renderBitmap));
                //    encoder.Save(fs);
                //}

                var bitmapImage = ToBitmapSource(renderBitmap);
                surface.Margin = new Thickness(x1, x2, x3, x4);
                return bitmapImage;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public BitmapImage ToBitmapSource(BitmapSource bs)
        {
            var bitmapImage = new BitmapImage();
            var bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(bs));

            using (var stream = new MemoryStream())
            {
                bitmapEncoder.Save(stream);
                stream.Seek(0, SeekOrigin.Begin);

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.W:
                        MainWindow.floatWindows.Remove(Title);
                        this.Close();
                        break;
                    case Key.S:
                        Save();
                        break;
                    default:
                        break;
                }
            }
        }

        private void Window_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                FaintFlg = !FaintFlg;
                this.Opacity = FaintFlg ? 0.2 : 1;
            }
        }

        object lockObj = new object();
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            lock (lockObj)
            {
                double scale = ScreenManager.GetScale(this);
                if (tempScale != scale)
                {
                    if (tempScale == 1 && scale == 1.5)
                    {
                        Width = Width * (2d / 3d);
                        Height = Height * (2d / 3d);
                    }
                    else if (tempScale == 1.5 && scale == 1)
                    {
                        Width = Width * (3d / 2d);
                        Height = Height * (3d / 2d);
                    }
                    else if (tempScale == 1 && scale == 1.25)
                    {
                        Width = Width * (4d / 5d);
                        Height = Height * (4d / 5d);
                    }
                    else if (tempScale == 1.25 && scale == 1)
                    {
                        Width = Width * (5d / 4d);
                        Height = Height * (5d / 4d);
                    }
                    else if (tempScale == 1.25 && scale == 1.5)
                    {
                        Width = Width * (4d / 5d);
                        Height = Height * (4d / 5d);
                    }
                    else if (tempScale == 1.5 && scale == 1.25)
                    {
                        Width = Width * (5d / 4d);
                        Height = Height * (5d / 4d);
                    }

                    tempScale = scale;
                }
            }
        }

        private void ClipboardCopyMi_Click(object sender, RoutedEventArgs e)
        {
            ImageToClipboard();
        }

        private void ClipboardPasteMi_Click(object sender, RoutedEventArgs e)
        {
            ImagePaste();
        }

        private void SaveMi_Click(object sender, RoutedEventArgs e)
        {
            Save();
        }

        public void Save()
        {
            var dialog = new SaveFileDialog();
            dialog.Filter = "PNGファイル (*.png)|*.png";
            //dialog.Filter = "PNGファイル (*.png)|*.png|JPGファイル (*.jpg)|*.jpg|すべて (*.*)|*.*";
            //dialog.InitialDirectory = Common.ImgFolder;
            dialog.DefaultExt = "*.png";
            dialog.FileName = Title + ".png";
            // ダイアログを表示する
            if (dialog.ShowDialog() == true)
            {
                string file = dialog.FileNames[0] ?? "";
                if (!string.IsNullOrEmpty(file))
                {
                    try
                    {
                        using (var fileStream = new FileStream(file, FileMode.Create))
                        {
                            BitmapSource img = ImageMerge(new List<BitmapSource>() { nowImg, canImg });
                            BitmapEncoder encoder = new PngBitmapEncoder();
                            encoder.Frames.Add(BitmapFrame.Create(img));
                            encoder.Save(fileStream);
                        }
                        MessageBox.Show("保存しました。");
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void CloseMi_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.floatWindows.Remove(Title);
            this.Close();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files;
                try
                {
                    files = (string[])e.Data.GetData(DataFormats.FileDrop);
                    if (files.Length == 0)
                    {
                        return;
                    }
                    string tempPath = "";
                    if (!Directory.Exists("Temp"))
                    {
                        Directory.CreateDirectory("Temp");
                    }
                    tempPath = @"Temp\" + Guid.NewGuid().ToString() + ".png";
                    File.Copy(files[0], tempPath, true);
                    NewOpen(tempPath);
                    e.Handled = true;
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void SaveBt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Save();
        }

        private void PinBt_MouseDown(object sender, MouseButtonEventArgs e)
        {
            PinFlg = !PinFlg;
            PinImg.Source = new BitmapImage(new Uri($@"/IMG/{(PinFlg ? "pin" : "noPin")}.png", UriKind.Relative));
            SetTopMost(PinFlg);
        }

        public BitmapImage GetRootImg(string fileName)
        {
            return new BitmapImage(new Uri(fileName, UriKind.Relative)); ;
        }
    }
}
