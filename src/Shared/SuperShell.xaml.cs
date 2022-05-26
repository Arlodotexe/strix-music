using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.Input;
using OwlCore;
using OwlCore.AbstractUI.ViewModels;
using OwlCore.Extensions;
using StrixMusic.Controls;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Services;
using StrixMusic.Services.CoreManagement;
using StrixMusic.Shared.ViewModels;
using Windows.ApplicationModel;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shared
{
    /// <summary>
    /// The SuperShell is a top-level overlay that will always show on top of all other shells. It provides various essential app functions, such as changing settings, setting your shell, viewing debug info, and managing cores.
    /// </summary>
    public sealed partial class SuperShell : UserControl
    {
        private readonly AppSettings _appSettings;
        private readonly ICoreManagementService _coreManagementService;
        private readonly SynchronizationContext _syncContext;
        private readonly LoadedServicesItemViewModel _addNewItem;
        private AdvancedAppSettingsPanel? _advancedSettings;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuperShell"/> class.
        /// </summary>
        public SuperShell(AppSettings appSettings, ICoreManagementService coreManagementService)
        {
            _appSettings = appSettings;
            _coreManagementService = coreManagementService;
            _syncContext = SynchronizationContext.Current ?? new SynchronizationContext();
            CancelAddNewCommand = new RelayCommand(() => IsShowingAddNew = false);
            CancelConfigCoreCommand = new RelayCommand(() => CurrentCoreConfig = null);
            InitializeComponent();

            var registry = CoreRegistry.MetadataRegistry;
            Guard.IsTrue(registry.Count > 0);

            foreach (var metadata in registry)
                AvailableServices.Add(new AvailableServicesItemViewModel(coreManagementService, metadata));

            ShellSelectorViewModel = new ShellSelectorViewModel(appSettings);
            _ = ShellSelectorViewModel.InitAsync();

            _addNewItem = new LoadedServicesItemViewModel(true, null);
            Services.Add(_addNewItem);

            // The instance is kept in memory and reused in xaml.
            // Loaded and Unloaded are called as needed when added to and removed from the visual tree,
            // but the constructor is only called once.
            Loaded += SuperShell_Loaded;
        }

        private void SuperShell_Loaded(object sender, RoutedEventArgs e)
        {
            AttachEvents();

            _advancedSettings = new AdvancedAppSettingsPanel(_appSettings);
            AdvancedSettings = new AbstractUICollectionViewModel(_advancedSettings);
        }

        private void AttachEvents()
        {
            Unloaded += OnUnloaded;
            var appFrame = Window.Current.GetAppFrame();
            Guard.IsNotNull(appFrame.ContentOverlay, nameof(appFrame.ContentOverlay));

            appFrame.ContentOverlay.Closed += ContentOverlay_Closed;
            _addNewItem.NewItemRequested += AddNewItem_NewItemRequested;
        }

        private void DetachEvents()
        {
            Unloaded -= OnUnloaded;
            var appFrame = Window.Current.GetAppFrame();
            Guard.IsNotNull(appFrame.ContentOverlay, nameof(appFrame.ContentOverlay));

            appFrame.ContentOverlay.Closed -= ContentOverlay_Closed;
            _addNewItem.NewItemRequested -= AddNewItem_NewItemRequested;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
            _advancedSettings?.Dispose();
        }

        /// <summary>
        /// The backing dependency property for <see cref="IsShowingAddNew"/>.
        /// </summary>
        public static readonly DependencyProperty IsShowingAddNewProperty =
            DependencyProperty.Register(nameof(IsShowingAddNew), typeof(bool), typeof(SuperShell), new PropertyMetadata(false));

        /// <summary>
        /// The backing dependency property for <see cref="LoadedCores"/>.
        /// </summary>
        public static readonly DependencyProperty LoadedCoresProperty =
            DependencyProperty.Register(nameof(LoadedCores), typeof(IReadOnlyList<ICore>), typeof(SuperShell), new PropertyMetadata(new ICore[] { }, (d, e) => d.Cast<SuperShell>().OnLoadedCoresChanged((IReadOnlyList<ICore>)e.OldValue, (IReadOnlyList<ICore>)e.NewValue)));

        /// <summary>
        /// The backing dependency property for <see cref="LoadedCores"/>.
        /// </summary>
        public static readonly DependencyProperty SelectedTabIndexProperty =
            DependencyProperty.Register(nameof(SelectedTabIndex), typeof(int), typeof(SuperShell), new PropertyMetadata(0));

        /// <summary>
        /// The backing dependency property for <see cref="LoadedCores"/>.
        /// </summary>
        public static readonly DependencyProperty CurrentCoreConfigProperty =
            DependencyProperty.Register(nameof(CurrentCoreConfig), typeof(CoreViewModel), typeof(SuperShell), new PropertyMetadata(null));

        /// <summary>
        /// The backing dependency property for <see cref="AdvancedSettings"/>.
        /// </summary>
        public static readonly DependencyProperty AdvancedSettingsProperty =
            DependencyProperty.Register(nameof(AdvancedSettings), typeof(AbstractUICollectionViewModel), typeof(SuperShell), new PropertyMetadata(null));

        /// <summary>
        /// The cores have been registered and loaded into the app.
        /// </summary>
        public IReadOnlyList<ICore> LoadedCores
        {
            get => (IReadOnlyList<ICore>)GetValue(LoadedCoresProperty);
            set => SetValue(LoadedCoresProperty, value);
        }

        /// <summary>
        /// If true, the user has selected to add a new item and the UI should reflect this.
        /// </summary>
        public bool IsShowingAddNew
        {
            get => (bool)GetValue(IsShowingAddNewProperty);
            set => SetValue(IsShowingAddNewProperty, value);
        }

        /// <summary>
        /// The index of the currently selected tab.
        /// </summary>
        public int SelectedTabIndex
        {
            get => (int)GetValue(SelectedTabIndexProperty);
            set => SetValue(SelectedTabIndexProperty, value);
        }

        /// <summary>
        /// If not null, the view should display the config panel for this core
        /// </summary>
        public CoreViewModel? CurrentCoreConfig
        {
            get => (CoreViewModel?)GetValue(CurrentCoreConfigProperty);
            set => SetValue(CurrentCoreConfigProperty, value);
        }

        /// <summary>
        /// The advanced settings for the app.
        /// </summary>
        public AbstractUICollectionViewModel? AdvancedSettings
        {
            get => (AbstractUICollectionViewModel)GetValue(AdvancedSettingsProperty);
            set => SetValue(AdvancedSettingsProperty, value);
        }

        /// <summary>
        /// When fired, the user has canceled adding a new item.
        /// </summary>
        public IRelayCommand CancelAddNewCommand { get; }

        /// <summary>
        /// When fired, the user has canceled configuring a core.
        /// </summary>
        public IRelayCommand CancelConfigCoreCommand { get; }

        /// <summary>
        /// The services that are available to be added.
        /// </summary>
        public ObservableCollection<AvailableServicesItemViewModel> AvailableServices { get; } = new();

        /// <summary>
        /// The loaded services displayed in the app.
        /// </summary>
        public ObservableCollection<LoadedServicesItemViewModel> Services { get; } = new();

        /// <summary>
        /// Gets the app version number.
        /// </summary>
        public string AppVersion => $"{Package.Current.Id.Version.Major}.{Package.Current.Id.Version.Minor}.{Package.Current.Id.Version.Build}";

        /// <inheritdoc cref="ShellSelectorViewModel" />
        public ShellSelectorViewModel ShellSelectorViewModel { get; }

        private void AttachEvents(ICore core)
        {
            core.CoreStateChanged += Core_CoreStateChanged;
        }

        private void DetachEvents(ICore core)
        {
            core.CoreStateChanged -= Core_CoreStateChanged;
        }

        private void AddNewItem_NewItemRequested(object? sender, EventArgs e)
        {
            IsShowingAddNew = true;
        }

        private void OnLoadedCoresChanged(IReadOnlyList<ICore> oldValue, IReadOnlyList<ICore> newValue)
        {
            var removedItems = oldValue.Where(x => !newValue.Contains(x)).ToArray();
            var addedItems = newValue.Where(x => !oldValue.Contains(x)).ToArray();

            foreach (var item in addedItems)
            {
                AttachEvents(item);

                var loadedServicesViewModel = new LoadedServicesItemViewModel(false, new CoreViewModel(item));
                loadedServicesViewModel.ConfigRequested += LoadedService_ConfigRequested;

                Services.Insert(0, loadedServicesViewModel);
            }

            if (addedItems.Length > 0)
                IsShowingAddNew = false;

            foreach (var item in removedItems)
            {
                DetachEvents(item);

                var serviceToRemove = Services.First(x => x.Core?.InstanceId == item.InstanceId);
                serviceToRemove.ConfigRequested -= LoadedService_ConfigRequested;

                Services.Remove(serviceToRemove);
            }

            foreach (var activeServices in Services)
                activeServices.CanDeleteCore = Services.Count <= 2;
        }

        private async void LoadedService_ConfigRequested(object? sender, EventArgs e)
        {
            Guard.IsNotNull(sender, nameof(sender));

            var viewModel = (LoadedServicesItemViewModel)sender;
            Guard.IsNotNull(viewModel.Core, nameof(viewModel.Core));

            if (viewModel.Core.CoreState == CoreState.Unloaded)
            {
                await viewModel.Core.InitAsync();
                return;
            }

            CurrentCoreConfig = viewModel.Core;
        }

        private void Core_CoreStateChanged(object? sender, CoreState e)
        {
            Guard.IsNotNull(sender, nameof(sender));
            var core = (ICore)sender;

            if (e is CoreState.Configured or CoreState.Unloaded)
            {
                if (CurrentCoreConfig?.InstanceId != core.InstanceId)
                    return;

                _syncContext.Post(_ => CurrentCoreConfig = null, null);
                return;
            }

            if (e == CoreState.NeedsConfiguration)
                _syncContext.Post(_ => CurrentCoreConfig = new CoreViewModel(core), null);
        }

        private void ContentOverlay_Closed(object? sender, EventArgs e)
        {
            CurrentCoreConfig = null;
            IsShowingAddNew = false;
        }
    }
}
