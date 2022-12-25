using System;
using StrixMusic.AppModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        private void DebugLogs_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= DebugLogs_Loaded;
            if (AppRoot != null && AppRoot.AppDebug != null && AppRoot.AppDebug.AppLogs != null)
                AppRoot.AppDebug.AppLogs.CollectionChanged += AppLogs_CollectionChanged;

            // Scrolls to the bottom.
            ScrollToBottom();
        }

        private async void AppLogs_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    ScrollToBottom();
                });
            }
        }

        private void ScrollToBottom() => PART_SvLogs.ChangeView(0.0f, double.MaxValue, 1.0f);
    }
}
