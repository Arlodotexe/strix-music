using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class LocalStorageCoreSettingsEditor : UserControl
    {
        /// <summary>
        /// Creates a new instance of <see cref="LocalStorageCoreSettingsEditor"/>.
        /// </summary>
        public LocalStorageCoreSettingsEditor()
        {
            this.InitializeComponent();

            Loaded += LocalStorageCoreSettingsEditor_Loaded;
            BrowseButton.Tapped += BrowseButton_Tapped;
        }

        private void BrowseButton_Tapped(object sender, TappedRoutedEventArgs e)
        { }

        private void LocalStorageCoreSettingsEditor_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= LocalStorageCoreSettingsEditor_Loaded;
            Unloaded += LocalStorageCoreSettingsEditor_Unloaded;
        }

        private void LocalStorageCoreSettingsEditor_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= LocalStorageCoreSettingsEditor_Unloaded;
        }

        /// <summary>
        /// Collection holding the data for <see cref="FolderPath" />
        /// </summary>
        public string FolderPath
        {
            get => (string)GetValue(FolderPathProperty);
            set => SetValue(FolderPathProperty, value);
        }

        /// <summary>
        /// Dependency property for <ses cref="FolderPath" />.
        /// </summary>
        public static readonly DependencyProperty FolderPathProperty =
            DependencyProperty.Register(nameof(FolderPath), typeof(string), typeof(LocalStorageCoreSettingsEditor), new PropertyMetadata(null, null));
    }
}
