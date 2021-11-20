using CaptureFloat.VIEW;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Threading;

namespace CaptureFloat.LOGIC
{
    public static class Common
    {

        public static bool IsScreenStreaming = false;
        public static bool IsCapture = false;
        public static bool IsOnlyClipboard = false;

        // Version
        public static string GetVersion(string prefix = "v", int step = 4)
        {
            var thisVer = Assembly.GetExecutingAssembly().GetName().Version;
            switch (step)
            {
                case 1:
                    return $"{prefix}{thisVer.Major}";
                case 2:
                    return $"{prefix}{thisVer.Major}.{thisVer.Minor}";
                case 3:
                    return $"{prefix}{thisVer.Major}.{thisVer.Minor}.{thisVer.Build}";
                default:
                    return $"{prefix}{thisVer}";
            }
        }

        // Root
        public static string RootFolder
        {
            get
            {
                return System.AppDomain.CurrentDomain.BaseDirectory;
            }
        }

        // Imgフォルダ
        public static string ImgFolder
        {
            get
            {
                string path = Path.Combine(RootFolder, "IMG");
                if (string.IsNullOrEmpty(App.Setting.ImgFolder))
                {
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                }
                else
                {
                    try
                    {
                        if (!Directory.Exists(App.Setting.ImgFolder))
                        {
                            Directory.CreateDirectory(App.Setting.ImgFolder);
                        }
                        path = App.Setting.ImgFolder;
                    }
                    catch (Exception)
                    {
                    }
                }
                return path;
            }
        }

        // ファイルロック確認
        public static bool IsFileLocked(string filePath)
        {
            try
            {
                using (File.Open(filePath, FileMode.Open)) { }
            }
            catch (IOException e)
            {
                var errorCode = Marshal.GetHRForException(e) & ((1 << 16) - 1);
                return errorCode == 32 || errorCode == 33;
            }

            return false;
        }

        // State変更
        public static void SetAllWindowState(WindowState state, string filter = null, string except = null)
        {
            foreach (Window win in App.Current.Windows)
            {
                try
                {
                    //Debug.WriteLine(win.ToString());
                    if (win.ToString().Contains("CaptureFloat")
                        && (filter == null ? true : win.ToString().Contains(filter))
                        && (except == null ? true : !win.ToString().Contains(except)))
                    {
                        win.WindowState = state;
                        switch (state)
                        {
                            case WindowState.Normal:
                                ProcessUtil.BringProcessToFront(new WindowInteropHelper(win).Handle);
                                ProcessUtil.ShowWindow(new WindowInteropHelper(win).Handle, ProcessUtil.SW_NORMAL);
                                ProcessUtil.BringProcessToFront(new WindowInteropHelper(win).Handle);
                                ProcessUtil.ShowWindow(new WindowInteropHelper(win).Handle, ProcessUtil.SW_NORMAL);
                                break;
                            case WindowState.Minimized:
                                ProcessUtil.ShowWindow(new WindowInteropHelper(win).Handle, ProcessUtil.SW_MINIMIZE);
                                break;
                            case WindowState.Maximized:
                                ProcessUtil.ShowWindow(new WindowInteropHelper(win).Handle, ProcessUtil.SW_MAXIMIZE);
                                break;
                            default:
                                break;
                        }
                    }
                    Thread.Sleep(10);
                }
                catch (Exception)
                {
                }
            }
        }

        // Invoke
        public static void Invoke(this Action act, FrameworkElement fe = null, bool exceptionThrowFlg = false)
        {
            try
            {
                Dispatcher dis;
                if (fe == null)
                {
                    dis = MainWindow.GetInstance().Dispatcher;
                }
                else
                {
                    dis = fe.Dispatcher;
                }

                if (dis == null || dis.CheckAccess())
                {
                    act();
                }
                else
                {
                    dis.Invoke(DispatcherPriority.Normal, act);
                }
            }
            catch (Exception)
            {
                if (exceptionThrowFlg)
                {
                    throw;
                }
            }
        }

        public static void OpenPath()
        {
            try
            {
                string path = Clipboard.GetText().Trim();
                Process proc = new Process
                {
                    StartInfo = new ProcessStartInfo(path)
                };
                proc.Start();
            }
            catch (Exception)
            {
            }
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        public static void GetComment()
        {
            Task.Factory.StartNew(() =>
            {
                Thread.Sleep(300);
                Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        string filename;
                        List<string> selected = new List<string>();
                        IntPtr handle = GetForegroundWindow();
                        foreach (SHDocVw.InternetExplorer window in new SHDocVw.ShellWindows())
                        {
                            if (window.HWND == (int)handle)
                            {
                                filename = Path.GetFileNameWithoutExtension(window.FullName).ToLower();
                                if (filename.ToLowerInvariant() == "explorer")
                                {
                                    Shell32.FolderItems items = ((Shell32.IShellFolderViewDual2)window.Document).SelectedItems();
                                    foreach (Shell32.FolderItem item in items)
                                    {
                                        selected.Add(item.Path);
                                    }
                                }
                                break;
                            }
                        }
                        if (selected.Count == 1)
                        {
                            string selectedFileFullName = selected[0];
                            string selectedFileName = Path.GetFileName(selectedFileFullName);
                            string dir = Path.GetDirectoryName(selectedFileFullName);

                            string commentFile = Path.Combine(dir, ".FileComment");
                            if (File.Exists(commentFile))
                            {
                                string[] lines = File.ReadAllLines(commentFile, Encoding.UTF8);
                                foreach (var line in lines)
                                {
                                    string[] item = line.Split('≣');
                                    if (item[0] == selectedFileName)
                                    {
                                        var cw1 = new CommentWindow(selectedFileName, item[1]);
                                        if (cw1.ShowDialog() == true)
                                        {
                                            ModifyComment(commentFile, selectedFileName, cw1.Comment);
                                        }
                                        return;
                                    }
                                }
                                {
                                    var cw2 = new CommentWindow(selectedFileName, "");
                                    if (cw2.ShowDialog() == true)
                                    {
                                        ModifyComment(commentFile, selectedFileName, cw2.Comment);
                                    }
                                }
                            }
                            else
                            {
                                var cw3 = new CommentWindow(selectedFileName, "");
                                if (cw3.ShowDialog() == true)
                                {
                                    ModifyComment(commentFile, selectedFileName, cw3.Comment);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                });
            });
        }


        public static void ModifyComment(string path, string fileName, string text)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, $"{fileName}≣{text}\n", Encoding.UTF8);
                File.SetAttributes(path, FileAttributes.Hidden);
            }
            else
            {
                string[] lines = File.ReadAllLines(path, Encoding.UTF8);
                bool IsExists = false;
                bool IsRemove = string.IsNullOrEmpty(text);
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] item = lines[i].Split('≣');
                    if (item[0] == fileName)
                    {
                        if (IsRemove)
                        {
                            var lineList = new List<string>(lines);
                            lineList.RemoveAt(i);
                            lines = lineList.ToArray();
                            break;
                        }
                        lines[i] = $"{fileName}≣{text}";
                        IsExists = true;
                    }
                }
                if (!IsExists && !IsRemove)
                {
                    Array.Resize(ref lines, lines.Length + 1);
                    lines[lines.Length - 1] = $"{fileName}≣{text}";
                }
                File.SetAttributes(path, FileAttributes.Normal);
                File.WriteAllLines(path, lines, Encoding.UTF8);
                File.SetAttributes(path, FileAttributes.Hidden);
            }
        }

    }
}
