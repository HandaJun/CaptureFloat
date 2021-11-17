using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using System.Windows.Media;

namespace CaptureFloat.LOGIC
{
    public class ScreenManager
    {
        public static IEnumerable<ScreenManager> AllScreens()
        {
            foreach (Screen screen in System.Windows.Forms.Screen.AllScreens)
            {
                yield return new ScreenManager(screen);
            }
        }

        public static ScreenManager GetScreenFrom(Window window)
        {
            WindowInteropHelper windowInteropHelper = new WindowInteropHelper(window);
            Screen screen = System.Windows.Forms.Screen.FromHandle(windowInteropHelper.Handle);
            ScreenManager screenManager = new ScreenManager(screen, window);
            return screenManager;
        }

        public static ScreenManager GetScreenFrom(System.Drawing.Point point)
        {
            //int x = (int)Math.Round(point.X);
            //int y = (int)Math.Round(point.Y);
            int x = point.X;
            int y = point.Y;

            // are x,y device-independent-pixels ??
            System.Drawing.Point drawingPoint = new System.Drawing.Point(x, y);
            Screen screen = System.Windows.Forms.Screen.FromPoint(drawingPoint);
            ScreenManager screenManager = new ScreenManager(screen);

            return screenManager;
        }

        public static ScreenManager Primary
        {
            get { return new ScreenManager(System.Windows.Forms.Screen.PrimaryScreen); }
        }

        private readonly Screen screen;

        private readonly Window win;

        private readonly double scale;

        internal ScreenManager(System.Windows.Forms.Screen screen, Window win = null)
        {
            this.screen = screen;
            this.win = win;
            this.scale = GetScale(win);
        }

        public Rect DeviceBounds
        {
            get { return this.GetRect(this.screen.Bounds); }
        }

        public Rect WorkingArea
        {
            get { return this.GetRect(this.screen.WorkingArea); }
        }

        private Rect GetRect(Rectangle value)
        {
            // should x, y, width, height be device-independent-pixels ??
            return new Rect
            {
                X = value.X,
                Y = value.Y,
                Width = value.Width,
                Height = value.Height
            };
        }

        public bool IsPrimary
        {
            get { return this.screen.Primary; }
        }

        public string DeviceName
        {
            get { return this.screen.DeviceName; }
        }

        public static double GetScale(Visual w = null)
        {
            if (w == null)
            {
                return 1;
            }

            //PresentationSource source = PresentationSource.FromVisual(w);
            //double dpiX = 1;
            //double dpiY = 1;
            //if (source != null)
            //{
            //    dpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11;
            //    dpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22;
            //}

            //DEVMODE dm = new DEVMODE();
            //dm.dmSize = (short)Marshal.SizeOf(typeof(DEVMODE));

            //EnumDisplaySettings(ToLPTStr(screen.DeviceName), ENUM_CURRENT_SETTINGS, ref dm);

            //Console.WriteLine($"Device: {screen.DeviceName}");
            //Console.WriteLine($"dmSize: {dm.dmSize}");
            //Console.WriteLine($"Real Resolution: {dm.dmPelsWidth}x{dm.dmPelsHeight}");
            //Console.WriteLine($"Virtual Resolution: {screen.Bounds.Width}x{screen.Bounds.Height}");
            //Console.WriteLine($"BitsPerPixel: {screen.BitsPerPixel}");

            //Console.WriteLine($"getScalingFactor: {getScalingFactor()}");
            var dpi = VisualTreeHelper.GetDpi(w).PixelsPerDip;
            //Console.WriteLine($"GetDpi {dpi}");
            //Console.WriteLine();
            return dpi;
        }

        public byte[] ToLPTStr(string str)
        {
            var lptArray = new byte[str.Length + 1];

            var index = 0;
            foreach (char c in str.ToCharArray())
                lptArray[index++] = Convert.ToByte(c);

            lptArray[index] = Convert.ToByte('\0');

            return lptArray;
        }

        [DllImport("gdi32.dll")]
        static extern int GetDeviceCaps(IntPtr hdc, int nIndex);
        public enum DeviceCap
        {
            VERTRES = 10,
            DESKTOPVERTRES = 117,
        }


        private float getScalingFactor()
        {
            Graphics g = Graphics.FromHwnd(IntPtr.Zero);
            IntPtr desktop = g.GetHdc();
            int LogicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.VERTRES);
            int PhysicalScreenHeight = GetDeviceCaps(desktop, (int)DeviceCap.DESKTOPVERTRES);
            float ScreenScalingFactor = (float)PhysicalScreenHeight / (float)LogicalScreenHeight;
            return ScreenScalingFactor; // 1.25 = 125%
        }


        const int ENUM_CURRENT_SETTINGS = -1;

        //[DllImport("user32.dll")]
        //public static extern bool EnumDisplaySettings(string lpszDeviceName, int iModeNum, ref DEVMODE lpDevMode);

        [DllImport("User32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern Boolean EnumDisplaySettings(
                                           byte[] lpszDeviceName,
                                           [param: MarshalAs(UnmanagedType.U4)]
                                            int iModeNum,
                                            [In, Out]
                                            ref DEVMODE lpDevMode);

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {
            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;
        }
    }
}
