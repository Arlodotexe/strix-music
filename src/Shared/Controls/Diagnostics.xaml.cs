using System;
using OwlCore.Storage;
using StrixMusic.AppModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Controls
{
    /// <summary>
    /// Responsible for the handling and displaying debug logs on UI.
    /// </summary>
    public sealed partial class Diagnostics : UserControl
    {
        /// <summary>
        /// Displays the debug logs.
        /// </summary>
        public Diagnostics()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The data root for this app instance.
        /// </summary>
        public AppDiagnostics? DiagnosticData
        {
            get => (AppDiagnostics?)GetValue(DiagnosticDataProperty);
            set => SetValue(DiagnosticDataProperty, value);
        }

        /// <summary>
        /// The backing dependency property for <see cref="DiagnosticData"/>.
        /// </summary>
        public static readonly DependencyProperty DiagnosticDataProperty =
            DependencyProperty.Register(nameof(DiagnosticData), typeof(AppDiagnostics), typeof(Diagnostics), new PropertyMetadata(null));

        private IFolder? AsFolder(object obj) => obj as IFolder;

        private bool IsFolder(object obj) => obj is IFolder;

        private bool And(bool val1, bool val2) => val1 && val2;

        private bool AndNot(bool val1, bool val2) => !(val1 && val2);

        private bool AllNotNullOrWhiteSpace(string value, string value2) => !string.IsNullOrWhiteSpace(value) && !string.IsNullOrWhiteSpace(value2);

        private bool IsNull(object? obj) => obj is null;

        private bool IsNotNull(object? obj) => obj is not null;

        private Visibility IsNullToVisibility(object? obj) => obj is null ? Visibility.Visible : Visibility.Collapsed;

        private Visibility IsNotNullToVisibility(object? obj) => obj is not null ? Visibility.Visible : Visibility.Collapsed;

        private bool InvertBool(bool val) => !val;

        private Visibility BoolToVisibility(bool val) => val ? Visibility.Visible : Visibility.Collapsed;

        private Visibility InvertBoolToVisibility(bool val) => !val ? Visibility.Visible : Visibility.Collapsed;

        private Uri StringToUri(string value) => new Uri(value);

        private void ManualCrash(object sender, RoutedEventArgs e) => throw new Exception("User manually invoked a crash via diagnostic settings.");
    }
}
