using System;
using System.Collections.Generic;
using System.Linq;
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
    /// CommentWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class CommentWindow : Window
    {
        public string Comment { get; set; }

        public CommentWindow(string title, string text)
        {
            InitializeComponent();
            Title = title;
            Comment = text.Replace("<br>", "\r\n");
            CommentTb.Text = Comment;
            CommentTb.SelectionStart = Comment.Length;
            CommentTb.Focus();
        }

        private void SaveBt_Click(object sender, RoutedEventArgs e)
        {
            Comment = CommentTb.Text.Replace("\r\n", "<br>");
            DialogResult = true;
        }

        private void CloseBt_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.S:
                        {
                            Comment = CommentTb.Text.Replace("\r\n", "<br>");
                            DialogResult = true;
                        }
                        break;
                }
            }

            if (e.Key == Key.Escape)
            {
                DialogResult = false;
            }
        }

        private void ClearBt_Click(object sender, RoutedEventArgs e)
        {
            Comment = "";
            DialogResult = true;
        }
    }
}
