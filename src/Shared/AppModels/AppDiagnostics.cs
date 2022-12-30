using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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
using StrixMusic.Settings;

namespace StrixMusic.AppModels
{
    /// <summary>
    /// Responsible for handling app debug and diagnostics.
    /// </summary>
    public partial class AppDiagnostics : ObservableObject
    {
        private readonly SynchronizationContext _syncContext;
        private readonly DispatcherTimer _memoryWatchTimer;
        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private readonly IModifiableFolder _dataFolder;

        private ContentDialog? _deleteUserDataDialog;
        
        [ObservableProperty] private DiagnosticSettings _settings;
        [ObservableProperty] private string? _windowSizeStr;
        [ObservableProperty] private string? _memoryUsage;
        [ObservableProperty] private string? _windowHeight;
        [ObservableProperty] private string? _windowWidth;
        [ObservableProperty] private string? _totalMemory;
        [ObservableProperty] private string? _expectedMemoryLimit;

        /// <summary>
        /// Holds the list of all logs.
        /// </summary>
        public ObservableCollection<string>? Logs { get; } = new();

        /// <summary>
        /// Creates a new instance of <see cref="AppDiagnostics"/>.
        /// </summary>
        public AppDiagnostics(IModifiableFolder dataFolder)
        {
            _syncContext = SynchronizationContext.Current ?? new();
            _settings = new DiagnosticSettings(dataFolder);
            _dataFolder = dataFolder;

            if (_settings.IsLoggingEnabled)
                Logger.MessageReceived += Logger_MessageReceived;
            
            _settings.PropertyChanged += SettingsOnPropertyChanged;
            Window.Current.SizeChanged += Current_SizeChanged;
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_MemoryChanged;
            MemoryManager.AppMemoryUsageDecreased += MemoryManager_MemoryChanged;

            UpdateMemoryUsage();
            SetWindowSize();

            _memoryWatchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            _memoryWatchTimer.Tick += DispatchTimer_Elapsed;
            _memoryWatchTimer.Start();
        }

        private void SettingsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_settings.IsLoggingEnabled))
            {
                if (_settings.IsLoggingEnabled)
                {
                    Logger.MessageReceived += Logger_MessageReceived;
                    Logger.LogInformation("Logger enabled");
                }
                else
                {
                    Logger.LogInformation("Logger disabled");
                    Logger.MessageReceived -= Logger_MessageReceived;
                }
            }
        }

        private void DispatchTimer_Elapsed(object sender, object e)
        {
            _memoryWatchTimer.Stop();
            UpdateMemoryUsage();
            _memoryWatchTimer.Start();
        }

        private void MemoryManager_MemoryChanged(object sender, object e) => UpdateMemoryUsage();

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
                var formattedLogMessage = LogFormatter.GetFormattedLogMessage(e);

                if (Logs == null)
                    return;

                await _syncContext.PostAsync(() =>
                {
                    Logs.Add(formattedLogMessage);
                    return Task.CompletedTask;
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
                Content = "Are you sure you want to delete user data? The app will restart.",
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

                var result = await CoreApplication.RequestRestartAsync(launchArguments: string.Empty);

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

                await ShowRetryContentDialogAsync($"Couldn't delete user data.", PerformUserDataDeletionCommand, ex);
            }
        }

        [RelayCommand]
        private async Task<AppRestartFailureReason> RestartAppAsync() => await CoreApplication.RequestRestartAsync("Application Restart Programmatically.");

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
                CloseButtonText = "Ignore",
                PrimaryButtonText = "Try again",
                PrimaryButtonCommand = retryCommand,
            };

            await retryConfirmationDialog.ShowAsync(ShowType.Interrupt);
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
