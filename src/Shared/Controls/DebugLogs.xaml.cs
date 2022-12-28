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
