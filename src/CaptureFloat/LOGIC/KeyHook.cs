using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Forms;

namespace CaptureFloat.LOGIC
{
    public class KeyHook
    {
        public static KeyboardHook _hook;

        public KeyHook()
        {
            _hook = new KeyboardHook();
            _hook.KeyDown += new KeyboardHook.HookEventHandler(OnHookKeyDown);
            _hook.KeyUp += new KeyboardHook.HookEventHandler(OnHookKeyUp);
        }

        public void OnHookKeyDown(object sender, HookEventArgs e)
        {
            //Debug.WriteLine($"Alt : {e.Alt} KeyDown : {e.Key}");

            if (altKeyFlg && !e.Control && !e.Shift)
            {
                if(e.Key == Keys.O)
                {
                    Common.Invoke(() => {
                        MainWindow.GetInstance().ImageOpen();
                    });
                }
                if (e.Key == Keys.I)
                {
                    Common.SetAllWindowState(WindowState.Normal, "FloatWindow", "MainWindow");
                }
            }
            
            if (e.Control && !e.Shift && !altKeyFlg)
            {
                if (e.Key == Keys.E)
                {
                    Common.OpenPath();
                }
            }

            if (e.Key == Keys.LMenu || e.Key == Keys.RMenu)
            {
                KeyDownAlt();
            }
            else
            {
                altKeyFlg = false;
            }
            
        }
        public void OnHookKeyUp(object sender, HookEventArgs e)
        {
            //Debug.WriteLine($"Alt : {e.Alt} KeyUp : {e.Key}");

            if (e.Key == Keys.LMenu || e.Key == Keys.RMenu)
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
                    Common.Invoke(() => {
                        MainWindow.GetInstance().CaptureStart();
                    });
                }
                altKeyFlg = false;
            }
        }

        
    }
}
