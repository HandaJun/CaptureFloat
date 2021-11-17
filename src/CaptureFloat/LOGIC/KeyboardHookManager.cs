using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaptureFloat.LOGIC
{
    class KeyboardHookManager
    {
        private static int distance = 10;

        public void KeyboardHookListenerOnKeyDown(object sender, KeyEventArgs e)
        {
            //if (e.KeyCode == Keys.PrintScreen)
            //{
            //    e.Handled = true;
            //}

            if (e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu)
            {
                KeyDownAlt();
            }
            else
            {
                altKeyFlg = false;
            }

            //if (e.Alt && e.KeyCode == System.Windows.Forms.Keys.S)
            //{
            //    Console.WriteLine("Alt + S");
            //    e.Handled = true;
            //}

            //if (altKeyFlg && !e.Control && !e.Shift)
            //{
            //    if (e.KeyCode == Keys.Oem3)
            //    {
            //        Common.Invoke(() => {
            //            MainWindow.GetInstance().ImageOpen();
            //        });ooo
            //        e.Handled = true;
            //    }
            //    if (e.KeyCode == Keys.Oem1)
            //    {
            //        Common.SetAllWindowState(System.Windows.WindowState.Normal, "FloatWindow", "MainWindow");
            //        e.Handled = true;
            //    }
            //}

            //Debug.WriteLine(e.KeyData.ToString());

            if (e.KeyData.ToString() == "Oemtilde, Alt")
            {
                Common.Invoke(() =>
                {
                    MainWindow.GetInstance().ImageOpen();
                });
                e.Handled = true;
            }

            if (e.KeyData.ToString() == "Oem1, Alt")
            {
                Common.SetAllWindowState(System.Windows.WindowState.Normal, "FloatWindow", "MainWindow");
                e.Handled = true;
            }

            if (e.KeyData.ToString() == "Z, Alt")
            {
                Common.OpenPath();
                e.Handled = true;
            }

            if (e.KeyData.ToString() == "F8")
            {
                Common.GetComment();
                //e.Handled = true;
            }

            if (MouseHookManager.IsMouseDown || Common.IsCapture)
            {

                switch (e.KeyData.ToString())
                {
                    case "Left, Shift":
                    case "A, Shift":
                        MoveLeft(distance);
                        e.Handled = true;
                        break;
                    case "Right, Shift":
                    case "D, Shift":
                        MoveRight(distance);
                        e.Handled = true;
                        break;
                    case "Up, Shift":
                    case "W, Shift":
                        MoveUp(distance);
                        e.Handled = true;
                        break;
                    case "Down, Shift":
                    case "S, Shift":
                        MoveDown(distance);
                        e.Handled = true;
                        break;
                    case "Left":
                    case "A":
                        MoveLeft();
                        e.Handled = true;
                        break;
                    case "Right":
                    case "D":
                        MoveRight();
                        e.Handled = true;
                        break;
                    case "Up":
                    case "W":
                        MoveUp();
                        e.Handled = true;
                        break;
                    case "Down":
                    case "S":
                        MoveDown();
                        e.Handled = true;
                        break;
                    default:
                        break;
                }

            }
        }

        public void KeyboardHookListenerOnKeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            //if (e.Alt && e.KeyCode == Keys.S)
            //{
            //    Console.WriteLine("Alt + S");
            //    e.Handled = true;
            //}

            if (e.KeyCode == Keys.LMenu || e.KeyCode == Keys.RMenu)
            {
                KeyUpAlt();
            }
            else
            {
                altKeyFlg = false;
            }
        }

        bool altKeyFlg = false;
        DateTime beforeAlt = DateTime.Now;

        public void KeyDownAlt()
        {
            altKeyFlg = true;
        }

        public void KeyUpAlt()
        {
            if (altKeyFlg)
            {
                DateTime now = DateTime.Now;
                TimeSpan gapTime = now - beforeAlt;
                //Debug.WriteLine("gapTime : " + gapTime.TotalMilliseconds.ToString());
                beforeAlt = now;
                if (gapTime.TotalMilliseconds < 500 && gapTime.TotalMilliseconds > 80)
                {
                    Common.Invoke(() =>
                    {
                        MainWindow.GetInstance().CaptureStart();
                    });
                }
                altKeyFlg = false;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MousePoint
        {
            public int X;
            public int Y;

            public MousePoint(int x, int y)
            {
                X = x;
                Y = y;
            }

            public System.Drawing.Point ToPoint()
            {
                return new System.Drawing.Point(X, Y);
            }

            public override string ToString()
            {
                return $"X : {X}, Y : {Y}";
            }
        }

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out MousePoint lpMousePoint);

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetCursorPos(int x, int y);

        public MousePoint GetCursorPosition()
        {
            MousePoint currentMousePoint;
            var gotPoint = GetCursorPos(out currentMousePoint);
            if (!gotPoint) { currentMousePoint = new MousePoint(0, 0); }
            return currentMousePoint;
        }

        public void SetCursorPosition(int x, int y)
        {
            SetCursorPos(x, y);
        }

        public void SetCursorPosition(System.Drawing.Point point)
        {
            SetCursorPos(point.X, point.Y);
        }

        public static void SetCursorPosition(MousePoint point)
        {
            SetCursorPos(point.X, point.Y);
        }

        // 右に移動
        public void MoveRight(int distance = 1)
        {
            var beforePosition = GetCursorPosition();
            beforePosition.X = beforePosition.X + distance;
            SetCursorPosition(beforePosition);
        }

        // 左に移動
        public void MoveLeft(int distance = 1)
        {
            var beforePosition = GetCursorPosition();
            beforePosition.X = beforePosition.X - distance;
            SetCursorPosition(beforePosition);
        }

        // 上に移動
        public void MoveUp(int distance = 1)
        {
            var beforePosition = GetCursorPosition();
            beforePosition.Y = beforePosition.Y - distance;
            SetCursorPosition(beforePosition);
        }

        // 下に移動
        public void MoveDown(int distance = 1)
        {
            var beforePosition = GetCursorPosition();
            beforePosition.Y = beforePosition.Y + distance;
            SetCursorPosition(beforePosition);
        }
    }
}
