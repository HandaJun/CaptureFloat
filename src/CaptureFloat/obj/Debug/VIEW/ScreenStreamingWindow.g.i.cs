﻿#pragma checksum "..\..\..\VIEW\ScreenStreamingWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "144BBAA1F6221715B6FD6C93CE8EF7355977D9119A236B99FEAA9C9826325DA3"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CaptureFloat.VIEW;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace CaptureFloat.VIEW {
    
    
    /// <summary>
    /// ScreenStreamingWindow
    /// </summary>
    public partial class ScreenStreamingWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 20 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBlock IpTb;
        
        #line default
        #line hidden
        
        
        #line 21 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CopyBt;
        
        #line default
        #line hidden
        
        
        #line 22 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button StopBt;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/CaptureFloat;component/view/screenstreamingwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            
            #line 8 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
            ((CaptureFloat.VIEW.ScreenStreamingWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
            ((CaptureFloat.VIEW.ScreenStreamingWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.Window_Closing);
            
            #line default
            #line hidden
            return;
            case 2:
            this.IpTb = ((System.Windows.Controls.TextBlock)(target));
            return;
            case 3:
            this.CopyBt = ((System.Windows.Controls.Button)(target));
            
            #line 21 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
            this.CopyBt.Click += new System.Windows.RoutedEventHandler(this.CopyBt_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.StopBt = ((System.Windows.Controls.Button)(target));
            
            #line 22 "..\..\..\VIEW\ScreenStreamingWindow.xaml"
            this.StopBt.Click += new System.Windows.RoutedEventHandler(this.StopBt_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
