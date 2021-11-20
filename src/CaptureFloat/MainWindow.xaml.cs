using CaptureFloat.LOGIC;
using CaptureFloat.VIEW;
using Microsoft.Win32;
using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CaptureFloat
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static MainWindow _instance = new MainWindow();
        private KeyboardHookListener keyboardHookListener;
        private static readonly KeyboardHookManager khm = new KeyboardHookManager();
        private static readonly MouseHookManager mhm = new MouseHookManager();

        public static double ScreenScale = 0;
        public static Rect ScreenRect = new Rect();
        public static double Magnification = 1;

        private MainWindow()
        {
            InitializeComponent();
            ShowInTaskbar = false;
        }

        public static MainWindow GetInstance()
        {
            return _instance;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            keyboardHookListener = new KeyboardHookListener(new GlobalHooker())
            {
                Enabled = true
            };
            keyboardHookListener.KeyDown += khm.KeyboardHookListenerOnKeyDown;
            keyboardHookListener.KeyUp += khm.KeyboardHookListenerOnKeyUp;

            mhm.Start();

            VersionMi_2.Header = $"CaptureFloat {Common.GetVersion("v", 6)}";

            ScreenRect = ScreenManager.GetScreenFrom(this).DeviceBounds;
            ScreenScale = ScreenManager.GetScale(this);
            if (ScreenScale == 1.25d)
            {
                MainWindow.Magnification = (4d / 5d);
            }
            else if (ScreenScale == 1.50d)
            {
                MainWindow.Magnification = (2d / 3d);
            }

            if (App.Setting.IsOnlyClipboard)
            {
                MenuOnlyClipboardBt.Icon = "✔";
            }
            else
            {
                MenuOnlyClipboardBt.Icon = null;
            }

            if (CheckStartup())
            {
                MenuStartupBt.Icon = "✔";
            }
            else
            {
                MenuStartupBt.Icon = null;
            }
        }

        public static List<CaptureHoldWindow> holdWindows = new List<CaptureHoldWindow>();
        public static List<DistanceWindow> disWindows = new List<DistanceWindow>();
        public static Dictionary<string, FloatWindow> floatWindows = new Dictionary<string, FloatWindow>();

        public void CaptureBt_Click(object sender, RoutedEventArgs e)
        {
            var screenImages = GetScreenImages();
            CaptureStart(screenImages);
        }

        /// <summary>
        /// 画面のイメージ取得
        /// </summary>
        /// <returns></returns>
        public System.Drawing.Bitmap[] GetScreenImages()
        {
            ScreenCapturer sc = new ScreenCapturer();
            Common.SetAllWindowState(WindowState.Minimized, null, "CaptureHoldWindow");
            System.Drawing.Bitmap[] bit = new System.Drawing.Bitmap[System.Windows.Forms.Screen.AllScreens.Length];
            for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
            {
                ScreenManager screen = new ScreenManager(System.Windows.Forms.Screen.AllScreens[i]);

                var bounds = screen.DeviceBounds;
                ScreenCapturer.Rect rect = new ScreenCapturer.Rect()
                {
                    Top = (int)bounds.Top,
                    Left = (int)bounds.Left,
                    Bottom = (int)bounds.Bottom,
                    Right = (int)bounds.Right,
                };
                bit[i] = sc.Capture(rect, enmScreenCaptureMode.Screen);
            }
            return bit;
        }

        /// <summary>
        /// キャプチャー開始
        /// </summary>
        /// <param name="bit"></param>
        public void CaptureStart(System.Drawing.Bitmap[] bit = null)
        {
            this.Visibility = Visibility.Hidden;
            Common.SetAllWindowState(WindowState.Minimized, "FloatWindow", "MainWindow");
            Task.Run(() =>
            {
                Thread.Sleep(100);
                this.Dispatcher.Invoke(() =>
                {
                    Common.IsCapture = true;
                    foreach (var hold in holdWindows)
                    {
                        hold.Close();
                    }
                    holdWindows.Clear();
                    for (int i = 0; i < System.Windows.Forms.Screen.AllScreens.Length; i++)
                    {
                        ScreenManager screen = new ScreenManager(System.Windows.Forms.Screen.AllScreens[i]);
                        var holdWindow = new CaptureHoldWindow(screen);
                        holdWindows.Add(holdWindow);
                        if (bit != null)
                        {
                            holdWindow.SetBackground(bit[i]);
                        }
                        holdWindow.Open();
                    }
                    Thread.Sleep(100);
                    Common.SetAllWindowState(WindowState.Normal, "CaptureHoldWindow", "MainWindow");
                });
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                mhm.Close();
            }
            catch (Exception)
            {
            }

            foreach (var fw in floatWindows.Values)
            {
                fw.Close();
            }
        }

        /// <summary>
        /// FloatのTopmost設定
        /// </summary>
        /// <param name="visible"></param>
        public void FloatWindowTopMost(bool visible)
        {
            foreach (var fw in floatWindows.Values)
            {
                fw.Topmost = visible;
            }
        }

        private void ExitBt_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// ファイル開く押下
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuOpenBt_Click(object sender, RoutedEventArgs e)
        {
            ImageOpen();
        }

        /// <summary>
        /// ファイル開く
        /// </summary>
        public void ImageOpen()
        {
            try
            {
                var dialog = new OpenFileDialog
                {
                    Filter = "PNGファイル (*.png)|*.png|JPGファイル (*.jpg)|*.jpg",
                    Multiselect = false,
                    InitialDirectory = Common.ImgFolder
                };
                // ダイアログを表示する
                if (dialog.ShowDialog() == true)
                {
                    string file = dialog.FileNames[0] ?? "";
                    if (!string.IsNullOrEmpty(file))
                    {
                        var img = GetImage(file);
                        Rect rect = new Rect(0, 0, img.Width, img.Height);
                        rect.X += (ScreenRect.Width - img.Width) / 2;
                        rect.Y += (ScreenRect.Height - img.Height) / 2;
                        new FloatWindow().Open(img, rect, new Rect(0, 0, 0, 0));
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public BitmapSource GetImage(string path)
        {
            BitmapImage bmp;
            try
            {
                if (Common.IsFileLocked(path))
                {
                    string tempPath = Common.ImgFolder + @"\" + Guid.NewGuid().ToString() + System.IO.Path.GetExtension(path);
                    File.Copy(path, tempPath);
                    path = tempPath;
                }
                bmp = new BitmapImage();
                bmp.BeginInit();
                bmp.UriSource = new Uri(path, UriKind.Relative);
                RenderOptions.SetBitmapScalingMode(bmp, BitmapScalingMode.NearestNeighbor);
                bmp.EndInit();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw ex;
            }
            return bmp.Clone();
        }

        private void MenuNormalBt_Click(object sender, RoutedEventArgs e)
        {
            Common.SetAllWindowState(WindowState.Normal, "FloatWindow", "MainWindow");
        }

        private void PathOpenBt_Click(object sender, RoutedEventArgs e)
        {
            Common.OpenPath();
        }

        private void TaskbarIcon_TrayLeftMouseDown(object sender, RoutedEventArgs e)
        {
            CaptureStart();
        }

        private void ScreenStreamingBt_Click(object sender, RoutedEventArgs e)
        {
            if (!Common.IsScreenStreaming)
            {
                ScreenStreamingWindow ss = new ScreenStreamingWindow();
                ss.Start();
            }
        }

        private void MenuOnlyClipboardBt_Click(object sender, RoutedEventArgs e)
        {
            App.Setting.IsOnlyClipboard = !App.Setting.IsOnlyClipboard;
            if (App.Setting.IsOnlyClipboard)
            {
                MenuOnlyClipboardBt.Icon = "✔";
            }
            else
            {
                MenuOnlyClipboardBt.Icon = null;
            }
            App.Setting.Save();
        }

        private void MenuImageSavePathChangeBt_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                App.Setting.ImgFolder = dialog.SelectedPath;
                App.Setting.Save();
                MessageBox.Show("保存先を変更しました。\n" + App.Setting.ImgFolder);
            }
        }

        private void MenuStartupBt_Click(object sender, RoutedEventArgs e)
        {
            if (CheckStartup())
            {
                InstallMeOffStartUp();
                MenuStartupBt.Icon = null;
            }
            else
            {
                InstallMeOnStartUp();
                MenuStartupBt.Icon = "✔";
            }
        }

        public void InstallMeOnStartUp()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.SetValue(curAssembly.GetName().Name, curAssembly.Location);
                MessageBox.Show("スタートアップに登録しました。");
            }
            catch { }
        }

        public void InstallMeOffStartUp()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                key.DeleteValue(curAssembly.GetName().Name);
                MessageBox.Show("スタートアップから削除しました。");
            }
            catch { }
        }

        public bool CheckStartup()
        {
            try
            {
                Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);
                Assembly curAssembly = Assembly.GetExecutingAssembly();
                var value = key.GetValue(curAssembly.GetName().Name);
                if (value != null)
                {
                    return true;
                }
            }
            catch { }
            return false;
        }

    }
}
