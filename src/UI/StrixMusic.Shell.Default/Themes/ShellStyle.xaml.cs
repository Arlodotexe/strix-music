using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using StrixMusic.Services.Navigation;
using StrixMusic.Shell.Default.Controls;
using StrixMusic.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Themes
{
    public sealed partial class ShellStyle : ResourceDictionary
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ShellStyle"/> class.
        /// </summary>
        public ShellStyle()
        {
            this.InitializeComponent();
        }
    }
}
