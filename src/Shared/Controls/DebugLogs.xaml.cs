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
        public AppDebug? AppDebug
        {
            get => (AppDebug?)GetValue(AppDebugProperty);
            set => SetValue(AppDebugProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="AppDebug"/>.
        /// </summary>
        public static readonly DependencyProperty AppDebugProperty =
            DependencyProperty.Register(nameof(AppDebug), typeof(AppDebug), typeof(DebugLogs), new PropertyMetadata(null));
    }
}
