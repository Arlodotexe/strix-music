using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using OwlCore.Diagnostics;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.System.Diagnostics;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.AppModels
{
    /// <summary>
    /// Responsible for handling debug information.
    /// </summary>
    public partial class AppDebug : ObservableObject
    {
        private SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        [ObservableProperty]
        private string? _windowSizeStr;

        [ObservableProperty]
        private string? _memoryUsage;

        private DispatcherTimer _memoryWatchTimer;

        /// <summary>
        /// Holds the list of all logs.
        /// </summary>
        public ObservableCollection<string>? AppLogs { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Creates a new instance of <see cref="AppDebug"/>.
        /// </summary>
        public AppDebug()
        {
            Logger.MessageReceived += Logger_MessageReceived;
            Window.Current.SizeChanged += Current_SizeChanged;
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_MemoryChanged;
            MemoryManager.AppMemoryUsageDecreased += MemoryManager_MemoryChanged;
            UpdateMemoryUsage();
            SetWindowSize();
            _memoryWatchTimer = new DispatcherTimer();
            _memoryWatchTimer.Interval = TimeSpan.FromSeconds(3);
            _memoryWatchTimer.Tick += DispatchTimer_Elapsed;
            _memoryWatchTimer.Start();
        }

        private void DispatchTimer_Elapsed(object sender, object e)
        {
            _memoryWatchTimer.Stop();
            UpdateMemoryUsage();
            _memoryWatchTimer.Start();
        }

        private void MemoryManager_MemoryChanged(object sender, object e)
        {
            UpdateMemoryUsage();
        }

        private void UpdateMemoryUsage()
        {
            var memoryUsed = MemoryManager.AppMemoryUsage;
            var totalMemory = MemoryManager.AppMemoryUsageLimit;
            MemoryUsage = $"App Memory used: {SizeSuffix((long)memoryUsed)}, Expected app memory usage limit: {SizeSuffix((long)totalMemory)}.";
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) => SetWindowSize();

        private void SetWindowSize()
        {
            WindowSizeStr = $"Current size is: Height: {Window.Current.Bounds.Height}, Width: {Window.Current.Bounds.Width}";
        }

        private async void Logger_MessageReceived(object sender, LoggerMessageEventArgs e)
        {
            await _semaphoreSlim.WaitAsync();

            var formatedMessage = LogFormatter.GetFormattedLogMessage(e);

            if (AppLogs == null)
                return;

            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
            () =>
            {
                AppLogs.Add(formatedMessage);
            });

            _semaphoreSlim.Release();
        }

        [RelayCommand]
        private async Task OpenLogFolderAsync()
        {
            var logsLocation = LogFormatter.LogFolderPath;
            var folder = await StorageFolder.GetFolderFromPathAsync(logsLocation);

            await Launcher.LaunchFolderAsync(folder);
        }

        [RelayCommand]
        private async Task DeleteUserDataAsync()
        {
            var contentDialog = new ContentDialog()
            {
                Title = "Delete user data",
                Content = "Are you sure you want to delete user data? If you hit yes, the app will restart.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                PrimaryButtonCommand = PerformUserDataDeletionCommand
            };

            await contentDialog.ShowAsync();
        }

        [RelayCommand]
        private async Task PerformUserDataDeletionAsync()
        {
            try
            {
                var localDataFolder = ApplicationData.Current.LocalFolder.Path;
                var localCache = ApplicationData.Current.LocalCacheFolder.Path;

                var folder = await StorageFolder.GetFolderFromPathAsync(localDataFolder);
                await folder.DeleteAsync();

                folder = await StorageFolder.GetFolderFromPathAsync(localCache);
                await folder.DeleteAsync();

                var result = await CoreApplication.RequestRestartAsync("Application Restart Programmatically.");

                if (result == AppRestartFailureReason.NotInForeground ||
                   result == AppRestartFailureReason.RestartPending ||
                   result == AppRestartFailureReason.Other)
                {
                    var msgBox = new MessageDialog("Restart Failed. Please restart the app manually.", result.ToString());
                    await msgBox.ShowAsync();
                }
            }
            catch (Exception)
            {
                var msgBox = new MessageDialog("Couldn't delete the user data.");
                await msgBox.ShowAsync();
            }
        }

        private string SizeSuffix(long value, int decimalPlaces = 1)
        {
            string[] sizeSuffixes = { "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };

            if (value < 0)
                return "-" + SizeSuffix(-value, decimalPlaces);

            var i = 0;
            var dValue = (decimal)value;
            while (Math.Round(dValue, decimalPlaces) >= 1000)
            {
                dValue /= 1024;
                i++;
            }

            return string.Format("{0:n" + decimalPlaces + "} {1}", dValue, sizeSuffixes[i]);
        }
    }
}
