using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Diagnostics;
using StrixMusic.AppModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace StrixMusic.Controls
{
    /// <summary>
    /// Responsible for the handling and displaying debug logs on UI.
    /// </summary>
    public sealed partial class DebugLogs : UserControl
    {
        /// <summary>
        /// Displays the debug logs.
        /// </summary>
        public DebugLogs()
        {
            this.InitializeComponent();
            Loaded += DebugLogs_Loaded;
        }

        private void DebugLogs_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DebugLogs_Loaded;
            if (AppRoot != null && AppRoot.AppLogs != null)
                AppRoot.AppLogs.CollectionChanged += AppLogs_CollectionChanged;

            PART_SvLogs.ChangeView(0.0f, double.MaxValue, 1.0f);
        }

        private void AppLogs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                PART_SvLogs.ChangeView(0.0f, double.MaxValue, 1.0f);
            }
        }

        /// <summary>
        /// The data root for this app instance.
        /// </summary>
        public AppRoot? AppRoot
        {
            get => (AppRoot?)GetValue(AppRootProperty);
            set => SetValue(AppRootProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="AppRoot"/>.
        /// </summary>
        public static readonly DependencyProperty AppRootProperty =
            DependencyProperty.Register(nameof(AppRoot), typeof(AppRoot), typeof(DebugLogs), new PropertyMetadata(null));
    }
}
