using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace CaptureFloat.LOGIC
{
    public static class ProcessUtil
    {

        public const uint MB_ICONWARNING = (uint)0x00000030L;
        public const uint MB_CANCELTRYCONTINUE = (uint)0x00000006L;
        public const uint MB_DEFBUTTON2 = (uint)0x00000100L;

        public const int SW_HIDE = 0;
        public const int SW_SHOW = 5;

        public const int HWND_TOPMOST = -1;
        public const int HWND_NOTOPMOST = -2;
        public const int SWP_NOMOVE = 0x0002;
        public const int SWP_NOSIZE = 0x0001;

        public const uint GA_ROOT = 2;

        public const uint WM_MOUSEWHEEL = 0x020A;
        public const UInt32 WM_VSCROLL = 0x0115;
        //public const int WM_MOUSEWHEEL = 0x020A;

        [StructLayout(LayoutKind.Sequential)]
        public struct Win32Point2
        {
            public int x;
            public int y;
        }

        /// <summary>
        /// 指定された文字列をウィンドウのタイトルとクラス名に含んでいるプロセスを閉じる
        /// </summary>
        public static void KillProcessesByWindow(string windowText, string className)
        {
            try
            {
                Process[] ps = GetProcessesByWindow(windowText, className);

                foreach (var item in ps)
                {
                    item.Kill();
                }

                if (ps.Length > 0)
                {
                    Thread.Sleep(1000);
                }
            }
            catch { }
        }

        /// <summary>
        /// 指定された文字列をウィンドウのタイトルとクラス名に含んでいるプロセスを
        /// すべて取得する。
        /// </summary>
        public static Process[] GetProcessesByWindow(
            string windowText, string className)
        {
            //検索の準備をする
            foundProcesses = new ArrayList();
            foundProcessIds = new ArrayList();
            searchWindowText = windowText;
            searchClassName = className;

            Process[] processList = null;
            try
            {
                EnumWindows(new EnumWindowsDelegate(EnumWindowCallBack), IntPtr.Zero);
                processList = (Process[])foundProcesses.ToArray(typeof(Process));
            }
            catch
            {
            }

            //結果を返す
            return processList;
        }

        private static string searchWindowText = null;
        private static string searchClassName = null;
        private static ArrayList foundProcessIds = null;
        private static ArrayList foundProcesses = null;

        private delegate bool EnumWindowsDelegate(IntPtr hWnd, IntPtr lparam);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private extern static bool EnumWindows(EnumWindowsDelegate lpEnumFunc,
            IntPtr lparam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd,
            StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetClassName(IntPtr hWnd,
            StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowThreadProcessId(
            IntPtr hWnd, out int lpdwProcessId);

        private static bool EnumWindowCallBack(IntPtr hWnd, IntPtr lparam)
        {
            if (searchWindowText != null)
            {
                int textLen = GetWindowTextLength(hWnd);
                if (textLen == 0)
                {
                    return true;
                }
                StringBuilder tsb = new StringBuilder(textLen + 1);
                GetWindowText(hWnd, tsb, tsb.Capacity);
                if (tsb.ToString().IndexOf(searchWindowText) < 0)
                {
                    return true;
                }
            }

            if (searchClassName != null)
            {
                StringBuilder csb = new StringBuilder(256);
                GetClassName(hWnd, csb, csb.Capacity);
                if (csb.ToString().IndexOf(searchClassName) < 0)
                {
                    return true;
                }
            }

            GetWindowThreadProcessId(hWnd, out int processId);
            if (!foundProcessIds.Contains(processId))
            {
                foundProcessIds.Add(processId);
                foundProcesses.Add(Process.GetProcessById(processId));
            }

            return true;
        }


        /// <summary>
        /// プロセスIDで
        /// </summary>
        /// <param name="pid"></param>
        /// <returns></returns>
        private static string FindIndexedProcessName(int pid)
        {
            var processName = Process.GetProcessById(pid).ProcessName;
            var processesByName = Process.GetProcessesByName(processName);
            string processIndexdName = null;

            try
            {
                for (var index = 0; index < processesByName.Length; index++)
                {
                    processIndexdName = index == 0 ? processName : processName + "#" + index;
                    var processId = new PerformanceCounter("Process", "ID Process", processIndexdName);
                    if ((int)processId.NextValue() == pid)
                    {
                        return processIndexdName;
                    }
                }
            }
            catch { }

            return processIndexdName;
        }

        /// <summary>
        /// プロセス名で親プロセス取得
        /// </summary>
        /// <param name="indexedProcessName"></param>
        /// <returns></returns>
        private static Process FindPidFromIndexedProcessName(string indexedProcessName)
        {
            Process proc = null;
            try
            {
                var parentId = new PerformanceCounter("Process", "Creating Process ID", indexedProcessName);
                proc = Process.GetProcessById((int)parentId.NextValue());
            }
            catch { }
            return proc;
        }

        /// <summary>
        /// プロセスの親プロセスを取得
        /// </summary>
        /// <param name="process"></param>
        /// <returns></returns>
        public static Process Parent(this Process process)
        {
            return FindPidFromIndexedProcessName(FindIndexedProcessName(process.Id));
        }


        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool AttachThreadInput(int idAttach, int idAttachTo, bool fAttach);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool SystemParametersInfo(uint uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        /// <summary>
        /// Windowフォームアクティブ化処理
        /// </summary>
        /// <param name="handle">フォームハンドル</param>
        /// <returns>true : 成功 / false : 失敗</returns>
        public static bool ForceActive(IntPtr handle)
        {
            const uint SPI_GETFOREGROUNDLOCKTIMEOUT = 0x2000;
            const uint SPI_SETFOREGROUNDLOCKTIMEOUT = 0x2001;
            const int SPIF_SENDCHANGE = 0x2;

            IntPtr dummy = IntPtr.Zero;
            IntPtr timeout = IntPtr.Zero;
            int foregroundID = GetWindowThreadProcessId(GetForegroundWindow(), out _);
            int targetID = GetWindowThreadProcessId(handle, out _);
            AttachThreadInput(targetID, foregroundID, true);
            SystemParametersInfo(SPI_GETFOREGROUNDLOCKTIMEOUT, 0, timeout, 0);
            SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, dummy, SPIF_SENDCHANGE);
            bool isSuccess = SetForegroundWindow(handle);
            SystemParametersInfo(SPI_SETFOREGROUNDLOCKTIMEOUT, 0, timeout, SPIF_SENDCHANGE);
            AttachThreadInput(targetID, foregroundID, false);
            return isSuccess;
        }

        [DllImport("User32.dll")]
        private static extern bool IsIconic(IntPtr handle);

        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);

        public const int SW_NORMAL = 1;
        public const int SW_MAXIMIZE = 3;
        public const int SW_MINIMIZE = 6;
        public const int SW_RESTORE = 9;

        public static void BringProcessToFront(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }
            SetForegroundWindow(handle);
        }

        public static void BringProcessToFront(IntPtr handle)
        {
            if (IsIconic(handle))
            {
                ShowWindow(handle, SW_RESTORE);
            }
            SetForegroundWindow(handle);
        }

        public static void SetMinimize(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            ShowWindow(handle, SW_MINIMIZE);
        }

        public static void SetMaximaize(Process process)
        {
            IntPtr handle = process.MainWindowHandle;
            ShowWindow(handle, SW_MAXIMIZE);
        }


        // アクティブウインドウのProcess取得
        public static Process GetForegroundProcess()
        {
            GetWindowThreadProcessId(GetForegroundWindow(), out int processId);
            return Process.GetProcessById(processId);
        }

        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr FindWindow(string strClassName, string strWindowName);

        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref Rect rectangle);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr hWnd, out System.Drawing.Rectangle lpRect);

        public struct Rect
        {
            public int Left { get; set; }
            public int Top { get; set; }
            public int Right { get; set; }
            public int Bottom { get; set; }
        }

        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern int MessageBox(int hWnd, String text, String caption, uint type);

        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern bool EnableWindow(IntPtr hwnd, bool enabled);

        [DllImport("user32.dll")]
        private static extern IntPtr GetWindow(IntPtr hWnd, uint uCmd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd,
            int hWndInsertAfter, int x, int y, int cx, int cy, int uFlags);

        public static bool ShowHideWindow(IntPtr hwnd, int nCmdShow)
        {
            return ShowWindow(hwnd, nCmdShow);
        }

        public static string GetClassName(IntPtr hWnd)
        {
            StringBuilder csb = new StringBuilder(256);
            GetClassName(hWnd, csb, csb.Capacity);
            return csb.ToString();
        }

        public static bool GetWindowZOrder(IntPtr hwnd, out int zOrder)
        {
            const uint GW_HWNDPREV = 3;
            const uint GW_HWNDLAST = 1;

            var lowestHwnd = GetWindow(hwnd, GW_HWNDLAST);

            var z = 0;
            var hwndTmp = lowestHwnd;
            while (hwndTmp != IntPtr.Zero)
            {
                if (hwnd == hwndTmp)
                {
                    zOrder = z;
                    return true;
                }

                hwndTmp = GetWindow(hwndTmp, GW_HWNDPREV);
                z++;
            }

            zOrder = int.MinValue;
            return false;
        }

        public delegate void WinEventDelegate(IntPtr hWinEventHook, uint eventType, IntPtr hwnd, int idObject, int idChild, uint dwEventThread, uint dwmsEventTime);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWinEventHook(uint eventMin, uint eventMax, IntPtr hmodWinEventProc, WinEventDelegate lpfnWinEventProc, uint idProcess, uint idThread, uint dwFlags);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern IntPtr WindowFromPoint(int xPoint, int yPoint);

        [System.Runtime.InteropServices.DllImport("user32.dll", ExactSpelling = true)]
        public static extern IntPtr GetAncestor(IntPtr hwnd, uint gaFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        public static IntPtr WindowFromPoint2(int x, int y)
        {
            return WindowFromPoint(x, y);
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool GetCursorPos(ref Win32Point pt);

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        public static Point GetMousePosition()
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return new Point(w32Mouse.X, w32Mouse.Y);
        }

        [DllImport("user32.dll", EntryPoint = "SystemParametersInfo")]
        public static extern bool SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [StructLayout(LayoutKind.Sequential)]
        public struct PROCESSENTRY32
        {
            public uint dwSize;
            public uint cntUsage;
            public uint th32ProcessID;
            public IntPtr th32DefaultHeapID;
            public uint th32ModuleID;
            public uint cntThreads;
            public uint th32ParentProcessID;
            public int pcPriClassBase;
            public uint dwFlags;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szExeFile;
        };

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateToolhelp32Snapshot(uint dwFlags, uint th32ProcessID);

        [DllImport("kernel32.dll")]
        static extern bool Process32First(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("kernel32.dll")]
        static extern bool Process32Next(IntPtr hSnapshot, ref PROCESSENTRY32 lppe);

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }

        public static float GetScalingFactor()
        {
            System.Drawing.Graphics g = System.Drawing.Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);

            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;

            return ScreenScalingFactor; // 1.25 = 125%
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr SendMessage(IntPtr hWnd, UInt32 Msg, Int32 wParam, ref Point lParam);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern bool GetGUIThreadInfo(uint idThread, ref GUITHREADINFO lpgui);

        public struct GUITHREADINFO
        {
            public int cbSize;
            public int flags;
            public IntPtr hwndActive;
            public IntPtr hwndFocus;
            public IntPtr hwndCapture;
            public IntPtr hwndMenuOwner;
            public IntPtr hwndMoveSize;
            public IntPtr hwndCaret;
            public System.Drawing.Rectangle rcCaret;
        }

    }
}
