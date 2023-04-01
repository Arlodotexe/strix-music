using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.UI.Xaml.Controls;
using NLog.Config;
using NLog.Targets;
using OwlCore.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Settings;
using Windows.ApplicationModel.Core;
using Windows.Storage;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.AppModels
{
    /// <summary>
    /// Responsible for handling app debug and diagnostics.
    /// </summary>
    public partial class AppDiagnostics : ObservableObject
    {
        private readonly SynchronizationContext _syncContext;

#if !__WASM__
        private readonly DispatcherTimer _memoryWatchTimer;
#endif

        private readonly SemaphoreSlim _semaphoreSlim = new(1, 1);
        private readonly IModifiableFolder _dataFolder;

        private ContentDialog? _deleteUserDataDialog;

        private DiagnosticSettings _settings;
        private string? _memoryUsage;
        private string? _totalMemory;
        private string? _expectedMemoryLimit;

        /// <summary>
        /// Creates a new instance of <see cref="AppDiagnostics"/>.
        /// </summary>
        public AppDiagnostics(IModifiableFolder dataFolder)
        {
            _syncContext = SynchronizationContext.Current ?? new();
            _settings = new DiagnosticSettings(dataFolder);
            _dataFolder = dataFolder;

            SetupNLog();
            SetupUnhandledExceptionLogger();

            // Setup default value
            if (_settings.IsLoggingEnabled)
            {
                // Event is connected for the lifetime of the application, unless disabled by user
                Logger.MessageReceived += Logger_MessageReceived;

                if (_settings.IsFirstChangeLoggingEnabled)
                {
                    AppDomain.CurrentDomain.FirstChanceException += OnCurrentDomainOnFirstChanceException;
                }
            }

            // Listen for config change
            _settings.PropertyChanged += SettingsOnPropertyChanged;

#if !__WASM__
            MemoryManager.AppMemoryUsageIncreased += MemoryManager_MemoryChanged;
            MemoryManager.AppMemoryUsageDecreased += MemoryManager_MemoryChanged;

            UpdateMemoryUsage();

            _memoryWatchTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };

            _memoryWatchTimer.Tick += DispatchTimer_Elapsed;
            _memoryWatchTimer.Start();
#endif

            Logger.LogInformation($"Diagnostics initialized");
        }

        /// <summary>
        /// A container for all settings related to debugging and diagnostics.
        /// </summary>
        public DiagnosticSettings Settings
        {
            get => _settings;
            set => SetProperty(ref _settings, value);
        }

        /// <summary>
        /// The current memory usage.
        /// </summary>
        public string? MemoryUsage
        {
            get => _memoryUsage;
            set => SetProperty(ref _memoryUsage, value);
        }

        /// <summary>
        /// The total memory available to the app.
        /// </summary>
        public string? TotalMemory
        {
            get => _totalMemory;
            set => SetProperty(ref _totalMemory, value);
        }

        /// <summary>
        /// Holds the list of all logs.
        /// </summary>
        public ObservableCollection<string>? Logs { get; } = new();

        private void SetupNLog()
        {
            // For privacy, logs should not leave local storage unless the user explicitly moves them. 
            var logPath = ApplicationData.Current.LocalCacheFolder.Path + @"\logs" + @"\${date:format=yyyy-MM-dd}.log";

            NLog.LogManager.Configuration = CreateConfig(shouldArchive: true);

            LoggingConfiguration CreateConfig(bool shouldArchive)
            {
                var config = new LoggingConfiguration();

                var fileTarget = new FileTarget("filelog")
                {
                    FileName = logPath,
                    EnableArchiveFileCompression = shouldArchive,
                    MaxArchiveDays = 7,
                    ArchiveNumbering = ArchiveNumberingMode.Sequence,
                    ArchiveOldFileOnStartup = shouldArchive,
                    KeepFileOpen = true,
                    OpenFileCacheTimeout = 10,
                    AutoFlush = false,
                    OpenFileFlushTimeout = 10,
                    ConcurrentWrites = false,
                    CleanupFileName = false,
                    Layout = "${message}",
                };

                config.AddTarget(fileTarget);
                config.AddRule(NLog.LogLevel.Debug, NLog.LogLevel.Fatal, "filelog");

                return config;
            }
        }

        private void SetupUnhandledExceptionLogger()
        {
            App.Current.UnhandledException += (sender, args) =>
            {
                var stack = new StackTrace();
                var stackFrames = stack.GetFrames();
                Logger.LogError($"Unhandled exception {string.Join<StackFrame>("    ", stackFrames)}", args.Exception);
            };
        }

        private void SettingsOnPropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(_settings.IsLoggingEnabled))
            {
                if (_settings.IsLoggingEnabled)
                {
                    Logger.MessageReceived -= Logger_MessageReceived;
                    Logger.MessageReceived += Logger_MessageReceived;
                    Logger.LogInformation("Logger enabled");
                }
                else
                {
                    Logger.MessageReceived -= Logger_MessageReceived;
                    Logger.LogInformation("Logger disabled");
                }
            }

            if (e.PropertyName == nameof(_settings.IsFirstChangeLoggingEnabled))
            {
                if (Settings.IsFirstChangeLoggingEnabled)
                {
                    AppDomain.CurrentDomain.FirstChanceException -= OnCurrentDomainOnFirstChanceException;
                    AppDomain.CurrentDomain.FirstChanceException += OnCurrentDomainOnFirstChanceException;
                    Logger.LogInformation("First chance logging enabled");
                }
                else
                {
                    AppDomain.CurrentDomain.FirstChanceException -= OnCurrentDomainOnFirstChanceException;
                    Logger.LogInformation("First chance logging disable");
                }
            }
        }

        private void OnCurrentDomainOnFirstChanceException(object sender, FirstChanceExceptionEventArgs args)
        {
            var stack = new StackTrace();
            var stackFrames = stack.GetFrames();
            Logger.LogError($"Unhandled exception {string.Join<StackFrame>("    ", stackFrames)}", args.Exception);
        }

        private void DispatchTimer_Elapsed(object? sender, object e)
        {
#if !__WASM__
            _memoryWatchTimer.Stop();
            UpdateMemoryUsage();
            _memoryWatchTimer.Start();
#endif
        }

        private void MemoryManager_MemoryChanged(object? sender, object e) => UpdateMemoryUsage();

        private void UpdateMemoryUsage()
        {
            var memoryUsed = MemoryManager.AppMemoryUsage;
            var totalMemory = MemoryManager.AppMemoryUsageLimit;

            MemoryUsage = SizeSuffix((long)memoryUsed);
            TotalMemory = SizeSuffix((long)totalMemory);
        }

        private async void Logger_MessageReceived(object? sender, LoggerMessageEventArgs e)
        {
            var message = $"{DateTime.UtcNow:O} [{e.Level}] [Thread {Thread.CurrentThread.ManagedThreadId}] L{e.CallerLineNumber} {Path.GetFileName(e.CallerFilePath)} {e.CallerMemberName} {e.Exception} {e.Message}";

            // Log to disk
            NLog.LogManager.GetLogger(string.Empty).Log(NLog.LogLevel.Info, message);

            // Re-emit for WebAssembly console output.
            Console.WriteLine(message);

            // Post to UI thread
            using (await _semaphoreSlim.DisposableWaitAsync())
            {
                if (Logs == null)
                    return;

                await _syncContext.PostAsync(() =>
                {
                    Logs.Add(message);
                    return Task.CompletedTask;
                });
            }
        }

        [RelayCommand]
        private async Task OpenLogFolderAsync()
        {
            var logsLocation = ApplicationData.Current.LocalCacheFolder.Path + @"\logs";
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
                var parent = (IModifiableFolder?)await ((IChildFolder)_dataFolder).GetParentAsync();
                Guard.IsNotNull(parent);

                await foreach (var item in parent.GetItemsAsync())
                {
                    await parent.DeleteAsync(item);
                }

                var result = await CoreApplication.RequestRestartAsync(launchArguments: string.Empty);

                if (result is AppRestartFailureReason.NotInForeground or AppRestartFailureReason.RestartPending or AppRestartFailureReason.Other)
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
                    Width = 300,
                    Spacing = 15,
                    Children =
                        {
                            new Expander
                            {
                                Width = 300,
                                Header = "View error",
                                ExpandDirection = ExpandDirection.Down,
                                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                                Content = new TextBlock { Text = $"Reason: {ex}", FontSize = 11, IsTextSelectionEnabled = true, TextWrapping = TextWrapping.WrapWholeWords },
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
