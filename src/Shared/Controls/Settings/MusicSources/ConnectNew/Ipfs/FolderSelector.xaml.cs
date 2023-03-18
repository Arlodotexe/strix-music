using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Storage;
using StrixMusic.Settings;
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

namespace StrixMusic.Controls.Settings.MusicSources.ConnectNew.Ipfs
{
    /// <summary>
    /// Folder selector for IPFS core/>
    /// </summary>
    [ObservableObject]
    public sealed partial class FolderSelector : Page
    {
        private SynchronizationContext _synchronizationContext;
        private ConnectNewMusicSourceNavigationParams? _param;
        [ObservableProperty] private IpfsCoreSettings? _settings = null;

        /// <summary>
        /// Creates a new instance of <see cref="FolderSelector"/>.
        /// </summary>
        public FolderSelector()
        {
            _synchronizationContext = SynchronizationContext.Current ?? new SynchronizationContext();
            this.InitializeComponent();
        }

        /// <inheritdoc />
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            var param = ((ConnectNewMusicSourceNavigationParams NavParams, IpfsCoreSettings Settings))e.Parameter;
            Guard.IsNotNull(param.NavParams);

            _param = param.NavParams;
            Settings = param.Settings;
        }

        [RelayCommand]
        private async Task PickFolderAsync(IFolder? folder)
        {
            if (folder is null)
                return;

            Guard.IsNotNull(_param?.AppRoot?.MusicSourcesSettings);
        }

        [RelayCommand]
        private async Task GetRootFolderAsync(CancellationToken cancellationToken)
        {
            Guard.IsNotNull(_param);
        }

        [RelayCommand]
        private void Cancel()
        {
            Guard.IsNotNull(_param);
            _param.SetupCompleteTaskCompletionSource.SetCanceled();
        }

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
    }
}
