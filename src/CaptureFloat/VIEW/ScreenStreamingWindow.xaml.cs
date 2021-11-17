using CaptureFloat.LOGIC;
using ScreenStreaming;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CaptureFloat.VIEW
{
    /// <summary>
    /// ScreenStreamingWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class ScreenStreamingWindow : Window
    {
        private ImageStreamingServer _Server;

        public ScreenStreamingWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        public void Start()
        {
            Show();
            string myIp = GetIp();
            if (string.IsNullOrEmpty(myIp))
            {
                MessageBox.Show("先にZIPANGUへ接続してください。");
                IpTb.Text = "接続失敗";
            }
            else
            {
                IpTb.Text = GetIp() + ":8080";
                Common.IsScreenStreaming = true;
                _Server = new ImageStreamingServer();
                _Server.Start(8080);
            }
        }

        private void CopyBt_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.Clear();
            Clipboard.SetText(IpTb.Text);
            MessageBox.Show("クリップボードにコピーしました。");
        }

        private void StopBt_Click(object sender, RoutedEventArgs e)
        {
            _Server.Dispose();
            Common.IsScreenStreaming = false;
            Hide();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _Server.Dispose();
            e.Cancel = true;
            Common.IsScreenStreaming = false;
            Hide();
        }

        private string GetIp()
        {
            // ホスト名を取得する
            string hostname = Dns.GetHostName();

            // ホスト名からIPアドレスを取得する
            IPAddress[] adrList = Dns.GetHostAddresses(hostname);
            foreach (IPAddress address in adrList)
            {
                string ip = address.ToString();
                if (ip.Length > 4)
                {
                    if (ip.StartsWith("10.") || ip.StartsWith("160."))
                    {
                        return ip;
                    }
                }
            }
            return "";
        }

    }
}
