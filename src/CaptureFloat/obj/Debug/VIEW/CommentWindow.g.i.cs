﻿#pragma checksum "..\..\..\VIEW\CommentWindow.xaml" "{8829d00f-11b8-4213-878b-770e8597ac16}" "4C2106EEC8C8565ECA9B62FD5835EE48E9FE3B94290618EE1B8E7345DB08B21D"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using CaptureFloat;
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
    /// CommentWindow
    /// </summary>
    public partial class CommentWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 120 "..\..\..\VIEW\CommentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TextBox CommentTb;
        
        #line default
        #line hidden
        
        
        #line 121 "..\..\..\VIEW\CommentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button SaveBt;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\..\VIEW\CommentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button CloseBt;
        
        #line default
        #line hidden
        
        
        #line 123 "..\..\..\VIEW\CommentWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Button ClearBt;
        
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
            System.Uri resourceLocater = new System.Uri("/CaptureFloat;component/view/commentwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\VIEW\CommentWindow.xaml"
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
            
            #line 8 "..\..\..\VIEW\CommentWindow.xaml"
            ((CaptureFloat.VIEW.CommentWindow)(target)).KeyDown += new System.Windows.Input.KeyEventHandler(this.Window_KeyDown);
            
            #line default
            #line hidden
            return;
            case 2:
            this.CommentTb = ((System.Windows.Controls.TextBox)(target));
            return;
            case 3:
            this.SaveBt = ((System.Windows.Controls.Button)(target));
            
            #line 121 "..\..\..\VIEW\CommentWindow.xaml"
            this.SaveBt.Click += new System.Windows.RoutedEventHandler(this.SaveBt_Click);
            
            #line default
            #line hidden
            return;
            case 4:
            this.CloseBt = ((System.Windows.Controls.Button)(target));
            
            #line 122 "..\..\..\VIEW\CommentWindow.xaml"
            this.CloseBt.Click += new System.Windows.RoutedEventHandler(this.CloseBt_Click);
            
            #line default
            #line hidden
            return;
            case 5:
            this.ClearBt = ((System.Windows.Controls.Button)(target));
            
            #line 123 "..\..\..\VIEW\CommentWindow.xaml"
            this.ClearBt.Click += new System.Windows.RoutedEventHandler(this.ClearBt_Click);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

