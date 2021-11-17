using MouseKeyboardActivityMonitor;
using MouseKeyboardActivityMonitor.WinApi;
using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CaptureFloat
{
    public class MouseHook
    {
        public static MouseHookListener mouseHookListener { get; set; } = new MouseHookListener(new GlobalHooker());

        public Action<object, MouseEventArgs> OnMouseDown { get; set; } = null;
        public Action<object, MouseEventArgs> OnMouseUp { get; set; } = null;
        public Action<object, MouseEventArgs> OnMouseMove { get; set; } = null;
        public Action<object, MouseEventArgs> OnMouseDoubleClick { get; set; } = null;
        public Action<object, MouseEventArgs> OnMouseWheel { get; set; } = null;

        public void Init()
        {
            mouseHookListener.Enabled = true;
            mouseHookListener.MouseDown += MouseHookListenerOnMouseDown;
            mouseHookListener.MouseUp += MouseHookListenerOnMouseUp;
            //mouseHookListener.MouseMove += MouseHookListenerOnMouseMove;
            //mouseHookListener.MouseDoubleClick += MouseHookListenerOnMouseDoubleClick;
            //mouseHookListener.MouseWheel += MouseHookListenerOnMouseWheel;

            //Log.Info("MouseHook 開始");
        }

        public void MouseHookListenerOnMouseDown(object sender, MouseEventArgs e)
        {
            if (OnMouseDown != null)
            {
                OnMouseDown(sender, e);
            }
        }

        public void MouseHookListenerOnMouseUp(object sender, MouseEventArgs e)
        {
            if (OnMouseUp != null)
            {
                OnMouseUp(sender, e);
            }
        }

        public void MouseHookListenerOnMouseMove(object sender, MouseEventArgs e)
        {
            if (OnMouseMove != null)
            {
                OnMouseMove(sender, e);
            }
        }

        public void MouseHookListenerOnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (OnMouseDown != null)
            {
                OnMouseDown(sender, e);
            }
        }

        public void MouseHookListenerOnMouseWheel(object sender, MouseEventArgs e)
        {
            if (OnMouseWheel != null)
            {
                OnMouseWheel(sender, e);
            }
        }

        public void Close()
        {
            mouseHookListener.Enabled = false;
            mouseHookListener.Dispose();
        }

    }
}
