using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace CaptureFloat
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static object Setting { get; internal set; }
        public static ApplicationSetting Setting { get; set; }
        public static string appUrl = @"\\kagami\Share_000661\Users\handa\APP\";

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            var proc = System.Diagnostics.Process.GetProcessesByName(System.Diagnostics.Process.GetCurrentProcess().ProcessName);
            //二重起動をチェックする
            if (proc.Length > 1)
            {
                Application.Current.Shutdown();
                return;
            }
            Setting = ApplicationSetting.Load();
            Setting.Save();

            if (!Update("CaptureFloat", Assembly.GetExecutingAssembly()))
            {
                CaptureFloat.MainWindow.GetInstance().Show();
            }
        }

        public bool Update(string fileNameStartWith, Assembly asm)
        {
            try
            {
                string checkPath = appUrl;
                string[] files = Directory.GetFiles(checkPath);
                bool isUpdate = false;
                string NewFilePath = "";
                foreach (string file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file);
                    if (fileName.StartsWith(fileNameStartWith))
                    {
                        //Version newVer = new Version(fileName.Substring(fileName.IndexOf("v") + 1));
                        var newVerInfo = FileVersionInfo.GetVersionInfo(file);
                        Version newVer = new Version(newVerInfo.FileVersion);
                        Version thisVer = asm.GetName().Version;
                        int result = newVer.CompareTo(thisVer);
                        if (result > 0)
                        {
                            NewFilePath = file;
                            isUpdate = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
                if (isUpdate)
                {
                    var result = MessageBox.Show("新しいバージョンがあります。アップデートしますか？", "Update", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        string exeFileName = Assembly.GetExecutingAssembly().CodeBase.Replace("file:///", string.Empty).Replace("/", "\\");
                        string exeFolder = System.IO.Path.GetDirectoryName(exeFileName);
                        //string newFileName = Path.GetFileName(NewFilePath);
                        string newFileName = $"{fileNameStartWith}.exe";
                        string batchName = $"Update{fileNameStartWith}.bat";

                        string batchCommands = "";
                        batchCommands += "@ECHO OFF\n";
                        //batchCommands += "echo Update.... \n";
                        batchCommands += "ping 127.0.0.1 > nul\n";
                        batchCommands += "echo j | del /F \"" + exeFileName + "\"\n";
                        batchCommands += "echo j | copy \"" + NewFilePath + "\" \"" + Path.Combine(exeFolder, newFileName) + "\"\n";
                        batchCommands += "mshta javascript:alert(\"Update Completed.\");close();\n";
                        //batchCommands += "echo j | start \"" + Path.Combine(exeFolder, newFileName) + "  -v runAs\"\n";
                        batchCommands += $"echo j | del {batchName}";
                        File.WriteAllText(batchName, batchCommands);
                        Process.Start(batchName);
                        this.Shutdown();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
            }
            return false;
        }
    }
}
