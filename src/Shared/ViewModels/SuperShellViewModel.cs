using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using Microsoft.Toolkit.Mvvm.Messaging;
using OwlCore;
using OwlCore.AbstractUI.Models;
using OwlCore.AbstractUI.ViewModels;
using OwlCore.Provisos;
using StrixMusic.Sdk;
using StrixMusic.Sdk.CoreManagement;
using StrixMusic.Sdk.Messages;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Services;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Core;
using Windows.Storage;

namespace StrixMusic.Shared.ViewModels
{
    /// <summary>
    /// ViewModel used by the <see cref="SuperShell"/>.
    /// </summary>
    public class SuperShellViewModel : ObservableRecipient, IRecipient<LogMessage>, IAsyncInit, IDisposable
    {
        private readonly MainViewModel _mainViewModel;
        private readonly LoadedServicesItemViewModel _addNewItem;
        private readonly ICoreManagementService _coreManagementService;
        private readonly AppSettings _settings;
        private readonly INotificationService _notificationService;
        private readonly ILocalizationService _localizationResourceLoader;
        private bool _isShowingAddNew;
        private int _selectedTabIndex;
        private CoreViewModel? _currentCoreConfig;
        private AbstractBoolean _loggingToggle;
        private ILogger<SuperShellViewModel> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShellViewModel"/> class.
        /// </summary>
        public SuperShellViewModel()
        {
            _mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();
            _coreManagementService = Ioc.Default.GetRequiredService<ICoreManagementService>();
            _settings = Ioc.Default.GetRequiredService<AppSettings>();
            _notificationService = Ioc.Default.GetRequiredService<INotificationService>();
            _localizationResourceLoader = Ioc.Default.GetRequiredService<ILocalizationService>();
            _logger = Ioc.Default.GetRequiredService<ILogger<SuperShellViewModel>>();

            ShellSelectorViewModel = new ShellSelectorViewModel();
            Services = new ObservableCollection<LoadedServicesItemViewModel>();
            AvailableServices = new ObservableCollection<AvailableServicesItemViewModel>();
            LogMessages = new ObservableCollection<LogMessage>();

            _loggingToggle = new AbstractBoolean("loggingToggle", "Use logging")
            {
                Subtitle = "Requires restart. When enabled, the app will save debug information to disk while running.",
            };

            AdvancedSettings = new AbstractUICollectionViewModel(CreateAdvancedSettings());

            // TODO nuke when switching to NavView for SuperShell.
            CancelAddNewCommand = new RelayCommand(() => IsShowingAddNew = false);
            CancelConfigCoreCommand = new RelayCommand(() => CurrentCoreConfig = null);
            ResetAppCommand = new AsyncRelayCommand(ResetAppAsync);

            foreach (var coreVm in _mainViewModel.Cores)
                Services.Add(new LoadedServicesItemViewModel(false, coreVm));

            _addNewItem = new LoadedServicesItemViewModel(true, null);
            Services.Add(_addNewItem);

            AttachEvents();
        }

        /// <inheritdoc/>
        public async Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return;

            _logger.LogInformation($"Started {nameof(InitAsync)}");

            IsInitialized = true;
            
            await _settings.LoadAsync(cancellationToken);
            _loggingToggle.State = _settings.IsLoggingEnabled;

            SetupCores();
            await ShellSelectorViewModel.InitAsync(cancellationToken);

            _logger.LogInformation($"Completed {nameof(InitAsync)}");
        }

        private void AttachEvents()
        {
            _addNewItem.NewItemRequested += AddNewItem_NewItemRequested;
            _mainViewModel.Cores.CollectionChanged += Cores_CollectionChanged;
            _loggingToggle.StateChanged += LoggingToggle_StateChanged;

            foreach (var loadedService in Services)
            {
                loadedService.ConfigRequested += LoadedService_ConfigRequested;
            }

            Guard.IsNotNull(CurrentWindow.AppFrame.ContentOverlay, nameof(CurrentWindow.AppFrame.ContentOverlay));
            CurrentWindow.AppFrame.ContentOverlay.Closed += ContentOverlay_Closed;
        }

        private void AttachEvents(ICore core)
        {
            core.CoreStateChanged += Core_CoreStateChanged;
        }

        private void DetachEvents()
        {
            _addNewItem.NewItemRequested -= AddNewItem_NewItemRequested;
            _mainViewModel.Cores.CollectionChanged -= Cores_CollectionChanged;
            _loggingToggle.StateChanged -= LoggingToggle_StateChanged;

            foreach (var loadedService in Services)
            {
                loadedService.ConfigRequested -= LoadedService_ConfigRequested;
            }

            Guard.IsNotNull(CurrentWindow.AppFrame.ContentOverlay, nameof(CurrentWindow.AppFrame.ContentOverlay));
            CurrentWindow.AppFrame.ContentOverlay.Closed -= ContentOverlay_Closed;
        }

        private void DetachEvents(ICore core)
        {
            core.CoreStateChanged -= Core_CoreStateChanged;
        }

        private async void LoadedService_ConfigRequested(object? sender, EventArgs e)
        {
            Guard.IsNotNull(sender, nameof(sender));

            var viewModel = (LoadedServicesItemViewModel)sender;
            Guard.IsNotNull(viewModel.Core, nameof(viewModel.Core));

            if (viewModel.Core.CoreState == CoreState.Unloaded)
            {
                await _mainViewModel.InitCore(viewModel.Core);
                return;
            }

            CurrentCoreConfig = viewModel.Core;
        }

        private async void ContentOverlay_Closed(object? sender, EventArgs e)
        {
            foreach (var core in _mainViewModel.Cores.ToArray())
            {
                if (core.CoreState == CoreState.Unloaded)
                {
                    await _coreManagementService.UnregisterCoreInstanceAsync(core.InstanceId);
                }
            }

            CurrentCoreConfig = null;
            IsShowingAddNew = false;
        }

        private async void LoggingToggle_StateChanged(object? sender, bool e)
        {
            _settings.IsLoggingEnabled = e;
            await _settings.SaveAsync();
        }

        private void AddNewItem_NewItemRequested(object? sender, EventArgs e)
        {
            IsShowingAddNew = true;
        }

        private async void Core_CoreStateChanged(object? sender, CoreState e)
        {
            Guard.IsNotNull(sender, nameof(sender));

            var core = (ICore)sender;
            var mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

            if (e == CoreState.Configured || e == CoreState.Unloaded)
            {
                await Threading.OnPrimaryThread(() => CurrentCoreConfig = null);
                return;
            }

            var relevantCore = mainViewModel.Cores.First(x => x.InstanceId == core.InstanceId);

            if (e == CoreState.NeedsConfiguration)
            {
                _ = Threading.OnPrimaryThread(() => CurrentCoreConfig = relevantCore);
            }

            if (CurrentCoreConfig?.InstanceId != relevantCore.InstanceId)
                return;
        }

        private void Cores_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (var item in e.NewItems)
                {
                    if (item is CoreViewModel coreVm)
                    {
                        AttachEvents(coreVm);

                        var loadedServicesViewModel = new LoadedServicesItemViewModel(false, coreVm);
                        loadedServicesViewModel.ConfigRequested += LoadedService_ConfigRequested;

                        Services.Insert(0, loadedServicesViewModel);
                    }
                }

                IsShowingAddNew = false;
            }

            if (e.OldItems != null)
            {
                foreach (var item in e.OldItems)
                {
                    if (item is CoreViewModel coreVm)
                    {
                        DetachEvents(coreVm);

                        var serviceToRemove = Services.First(x => x.Core?.InstanceId == coreVm.InstanceId);
                        serviceToRemove.ConfigRequested -= LoadedService_ConfigRequested;

                        Services.Remove(serviceToRemove);
                    }
                }
            }
        }

        /// <summary>
        /// The loaded services displayed in the app.
        /// </summary>
        public ObservableCollection<LoadedServicesItemViewModel> Services { get; }

        /// <summary>
        /// The services that are available to be added.
        /// </summary>
        public ObservableCollection<AvailableServicesItemViewModel> AvailableServices { get; }

        /// <summary>
        /// Messages logged by the app for debug purposes. 
        /// </summary>
        public ObservableCollection<LogMessage> LogMessages { get; set; }

        /// <summary>
        /// The advanced settings for the app.
        /// </summary>
        public AbstractUICollectionViewModel AdvancedSettings { get; }

        /// <inheritdoc cref="ShellSelectorViewModel" />
        public ShellSelectorViewModel ShellSelectorViewModel { get; }

        /// <summary>
        /// Gets the app version number.
        /// </summary>
        public string AppVersion => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

        /// <summary>
        /// If true, the user has selected to add a new item and the UI should reflect this.
        /// </summary>
        public bool IsShowingAddNew
        {
            get => _isShowingAddNew;
            set => SetProperty(ref _isShowingAddNew, value);
        }

        /// <summary>
        /// If not null, the view should display configuration
        /// </summary>
        public CoreViewModel? CurrentCoreConfig
        {
            get => _currentCoreConfig;
            set => SetProperty(ref _currentCoreConfig, value);
        }

        /// <summary>
        /// The index of the currently selected tab.
        /// </summary>
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetProperty(ref _selectedTabIndex, value);
        }

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// When fired, the user has canceled adding a new item.
        /// </summary>
        public IRelayCommand CancelAddNewCommand { get; }

        /// <summary>
        /// When fired, the user has canceled configuring a core.
        /// </summary>
        public IRelayCommand CancelConfigCoreCommand { get; }

        /// <summary>
        /// A command that resets the application.
        /// </summary>
        public IAsyncRelayCommand ResetAppCommand { get; }

        private Task ResetAppAsync()
        {
            var taskCompletionSource = new TaskCompletionSource<object?>();
            var confirmButton = new AbstractButton("confirmButton", "Confirm");
            var progressIndicator = new AbstractProgressIndicator("progressIndicator", isIndeterminate: true);
            var confirmationUI = new AbstractUICollection("confirmAppResetUI")
            {
                Title = "Are you sure?",
                Subtitle = "This will wipe all data and restart the app",
            };

            var notification = _notificationService.RaiseNotification(confirmationUI);

            confirmationUI.Add(confirmButton);
            confirmButton.Clicked += OnConfirmButtonClicked;
            notification.Dismissed += OnNotificationDismissed;

            void OnNotificationDismissed(object? sender, EventArgs e)
            {
                notification.Dismissed -= OnNotificationDismissed;
                confirmButton.Clicked -= OnConfirmButtonClicked;
                taskCompletionSource.SetResult(null);
            }

            async void OnConfirmButtonClicked(object? sender, EventArgs e)
            {
                notification.Dismissed -= OnNotificationDismissed;
                confirmButton.Clicked -= OnConfirmButtonClicked;

                notification.Dismiss();

                confirmationUI.Title = "Please wait.";
                confirmationUI.Subtitle = "Wipe in progress...";
                confirmationUI.Remove(confirmButton);
                confirmationUI.Add(progressIndicator);

                var progressNotification = _notificationService.RaiseNotification(confirmationUI);

                await PeformNuke();

                progressNotification.Dismiss();
                taskCompletionSource.SetResult(null);
            }

            async Task PeformNuke()
            {
                await EmptyFolder(ApplicationData.Current.LocalFolder);
                await EmptyFolder(ApplicationData.Current.LocalCacheFolder);
                await EmptyFolder(ApplicationData.Current.RoamingFolder);

                ApplicationData.Current.LocalSettings.Values.Clear();
                ApplicationData.Current.RoamingSettings.Values.Clear();

#if NETFX_CORE
                await CoreApplication.RequestRestartAsync(string.Empty);
#else
                _notificationService.RaiseNotification("Reset complete.", "Please restart the app");
#endif

                async Task EmptyFolder(IStorageFolder folder)
                {
                    IReadOnlyList<IStorageItem>? items = null;

                    try
                    {
                        items = await folder.GetItemsAsync();
                    }
                    catch
                    {
                        return;
                    }

                    foreach (var item in items)
                    {
                        try
                        {
                            await item.DeleteAsync();
                        }
                        catch
                        {
                            /* ignored */
                        }
                    }
                }
            }

            return taskCompletionSource.Task;
        }

        private AbstractUICollection CreateAdvancedSettings()
        {
            return new AbstractUICollection("advancedSettings")
            {
                CreateAdvancedSettings_Logging(),
                CreateAdvancedSettings_Recovery(),
            };
        }

        private AbstractUICollection CreateAdvancedSettings_Logging()
        {
            var openLogFolderButton = new AbstractButton("openLogFolder", "View logs");
            openLogFolderButton.Clicked += OpenLogFolderButton_Clicked;

            return new AbstractUICollection("loggingSettings")
            {
                Title = "Logging",
                Items = new List<AbstractUIElement> { _loggingToggle, openLogFolderButton }
            };
        }

        private async void OpenLogFolderButton_Clicked(object? sender, EventArgs e)
        {
            var logFolder = await ApplicationData.Current.LocalCacheFolder.CreateFolderAsync("Logs", CreationCollisionOption.OpenIfExists);

            await Windows.System.Launcher.LaunchFolderAsync(logFolder).AsTask();
        }

        private AbstractUICollection CreateAdvancedSettings_Recovery()
        {
            var resetButton = new AbstractButton("resetButton", "Reset everything");
            resetButton.Clicked += ResetButton_Clicked;

            return new AbstractUICollection("recoverySettings")
            {
                Title = "App recovery",
                Items = new List<AbstractUIElement> { resetButton }
            };
        }

        private void ResetButton_Clicked(object? sender, EventArgs e)
        {
            _ = ResetAppAsync();
        }

        /// <inheritdoc/>
        public void Receive(LogMessage message)
        {
            LogMessages.Add(message);
        }

        private void SetupCores()
        {
            var registry = CoreRegistry.MetadataRegistry;
            _logger.LogInformation($"Setting up {nameof(CoreRegistry)}. Total {registry.Count} items.");

            foreach (var metadata in registry)
            {
                _logger.LogInformation($"Adding {metadata.Id} to {nameof(AvailableServices)} (contains {AvailableServices.Count} items)");
                AvailableServices.Add(new AvailableServicesItemViewModel(metadata)); // Adding to this ObservableCollect kills the thread with no thrown exception?
            }

            foreach (var core in _mainViewModel.Cores)
                AttachEvents(core);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            DetachEvents();

            foreach (var core in _mainViewModel.Cores)
                DetachEvents(core);
        }
    }
}
