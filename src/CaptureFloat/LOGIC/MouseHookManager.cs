using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CaptureFloat
{
    public class MouseHookManager
    {
        private static MouseHook mh = null;
        public static bool IsMouseDown = false;
        private object lockObj = new object();

        public void Start()
        {
            mh = new MouseHook();
            mh.OnMouseDown = MouseHookListenerOnMouseDown;
            mh.OnMouseUp = MouseHookListenerOnMouseUp;
            mh.Init();
        }

        public void Close()
        {
            mh.Close();
        }

        public void MouseHookListenerOnMouseDown(object sender, MouseEventArgs e)
        {
            IsMouseDown = true;
        }

        public void MouseHookListenerOnMouseUp(object sender, MouseEventArgs e)
        {
            IsMouseDown = false;
        }

    }
}
