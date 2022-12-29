using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml.Controls;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using Windows.ApplicationModel.Core;
using Windows.Foundation;
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
        private readonly IModifiableFolder _dataFolder;

        [ObservableProperty]
        private string? _windowSizeStr;

        [ObservableProperty]
        private string? _memoryUsage;

        [ObservableProperty]
        private string? _windowHeight;

        [ObservableProperty]
        private string? _windowWidth;

        [ObservableProperty]
        private string? _totalMemory;

        [ObservableProperty]
        private string? _expectedMemoryLimit;

        private DispatcherTimer _memoryWatchTimer;
        private ContentDialog? _deleteUserDataDialog;

        /// <summary>
        /// Holds the list of all logs.
        /// </summary>
        public ObservableCollection<string>? AppLogs { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Creates a new instance of <see cref="AppDebug"/>.
        /// </summary>
        public AppDebug(IModifiableFolder folder)
        {
            _dataFolder = folder;
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
            MemoryUsage = SizeSuffix((long)memoryUsed);
            TotalMemory = SizeSuffix((long)totalMemory);
        }

        private void Current_SizeChanged(object sender, Windows.UI.Core.WindowSizeChangedEventArgs e) => SetWindowSize();

        private void SetWindowSize()
        {
            WindowHeight = Window.Current.Bounds.Height.ToString();
            WindowWidth = Window.Current.Bounds.Width.ToString();
        }

        private async void Logger_MessageReceived(object sender, LoggerMessageEventArgs e)
        {
            using (await _semaphoreSlim.DisposableWaitAsync())
            {
                var formatedMessage = LogFormatter.GetFormattedLogMessage(e);

                if (AppLogs == null)
                    return;

                await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () =>
                {
                    AppLogs.Add(formatedMessage);
                });
            }
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
            _deleteUserDataDialog = new ContentDialog()
            {
                Title = "Delete user data",
                Content = "Are you sure you want to delete user data? If you hit yes, the app will restart.",
                PrimaryButtonText = "Yes",
                SecondaryButtonText = "No",
                PrimaryButtonCommand = PerformUserDataDeletionCommand
            };

            await _deleteUserDataDialog.ShowAsync(ShowType.Interrupt);
        }

        [RelayCommand]
        private async Task PerformUserDataDeletionAsync()
        {
            try
            {
                await foreach (var item in _dataFolder.GetItemsAsync())
                {
                    await _dataFolder.DeleteAsync(item);
                }

                var result = await CoreApplication.RequestRestartAsync("Application Restart Programmatically.");

                if (result == AppRestartFailureReason.NotInForeground ||
                   result == AppRestartFailureReason.RestartPending ||
                   result == AppRestartFailureReason.Other)
                {
                    await ShowRetryContentDialogAsync($"Restart Failed. Please restart the app manually.", RestartAppCommand, null);
                }
            }
            catch (Exception ex)
            {
                Guard.IsNotNull(_deleteUserDataDialog);

                await ShowRetryContentDialogAsync($"Couldn't delete the user data.", PerformUserDataDeletionCommand, ex);
            }
        }

        private async Task ShowRetryContentDialogAsync(string error, ICommand retryCommand, Exception? ex)
        {
            Logger.LogError(error, ex);
            var retryConfirmationDialog = new ContentDialog
            {
                Title = error,
                Content = new StackPanel
                {
                    Width = 275,
                    Spacing = 15,
                    Children =
                        {
                            new Expander
                            {
                                Width = 275,
                                Header = "View error",
                                ExpandDirection = ExpandDirection.Down,
                                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                Content = new TextBlock { Text = $"Reason: {ex}", FontSize = 11, IsTextSelectionEnabled = true },
                            },
                        },
                },
                CloseButtonText = "Ignore, remove source",
                PrimaryButtonText = "Try again",
                PrimaryButtonCommand = retryCommand,
            };

            await retryConfirmationDialog.ShowAsync(ShowType.Interrupt);
        }

        [RelayCommand]
        private async Task<AppRestartFailureReason> RestartAppAsync() => await CoreApplication.RequestRestartAsync("Application Restart Programmatically.");

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
