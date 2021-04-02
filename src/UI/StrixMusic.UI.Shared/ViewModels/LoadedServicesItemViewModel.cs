﻿using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk;
using StrixMusic.Sdk.Services;
using StrixMusic.Sdk.Services.Notifications;
using StrixMusic.Sdk.ViewModels;

namespace StrixMusic.Shared.ViewModels
{
    /// <summary>
    /// Used to display an item in the service section of <see cref="SuperShell"/>.
    /// </summary>
    public class LoadedServicesItemViewModel : ObservableObject, IDisposable
    {
        private readonly MainViewModel _mainViewModel;
        private bool _canDeleteCore;

        /// <summary>
        /// Creates a new instance of <see cref="LoadedServicesItemViewModel"/>.
        /// </summary>
        /// <param name="isAddItem">Indicates that selecting this item will trigger adding a new core.</param>
        /// <param name="core">The core associated with this item, if applicable.</param>
        public LoadedServicesItemViewModel(bool isAddItem, CoreViewModel? core)
        {
            _mainViewModel = Ioc.Default.GetRequiredService<MainViewModel>();

            CanDeleteCore = _mainViewModel.Cores.Count > 1;
            IsAddItem = isAddItem;
            Core = core;

            AddNewItemCommand = new RelayCommand(AddNewItem);
            DeleteCoreCommand = new AsyncRelayCommand(DeleteCore);
            ConfigureCoreCommand = new RelayCommand(ConfigureCore);

            AttachEvents();
        }

        private void AttachEvents()
        {
            _mainViewModel.Cores.CollectionChanged += Cores_CollectionChanged;
        }

        private void DetachEvents()
        {
            _mainViewModel.Cores.CollectionChanged -= Cores_CollectionChanged;
        }

        private void Cores_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            CanDeleteCore = _mainViewModel.Cores.Count > 1;
        }

        /// <summary>
        /// The core associated with this item, if applicable.
        /// </summary>
        public CoreViewModel? Core { get; }

        /// <summary>
        /// Indicates if selecting this item will trigger adding a new core.
        /// </summary>
        public bool IsAddItem { get; }

        /// <summary>
        /// Indicates if this core can be removed.
        /// </summary>
        public bool CanDeleteCore
        {
            get => _canDeleteCore;
            set => SetProperty(ref _canDeleteCore, value);
        }

        /// <summary>
        /// Executed when the user wants to add a new service.
        /// </summary>
        public IRelayCommand AddNewItemCommand { get; }

        /// <summary>
        /// Executed when the user wants to delete this <see cref="Core"/>.
        /// </summary>
        public IAsyncRelayCommand DeleteCoreCommand { get; }

        /// <summary>
        /// Executed when the user wants to set settings on the core.
        /// </summary>
        public IRelayCommand ConfigureCoreCommand { get; }

        /// <summary>
        /// Raised when the user requests to add a new service.
        /// </summary>
        public event EventHandler? NewItemRequested;

        /// <summary>
        /// Raised when the user requests configuration on the core.
        /// </summary>
        public event EventHandler? ConfigRequested;

        private void AddNewItem()
        {
            NewItemRequested?.Invoke(this, EventArgs.Empty);
        }

        private Task DeleteCore()
        {
            Guard.IsNotNull(Core, nameof(Core));

            var coreManagementService = Ioc.Default.GetRequiredService<ICoreManagementService>();

            return coreManagementService.UnregisterCoreInstanceAsync(Core.InstanceId);
        }

        private void ConfigureCore()
        {
            ConfigRequested?.Invoke(this, EventArgs.Empty);
        }

        private void ReleaseUnmanagedResources()
        {
            DetachEvents();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~LoadedServicesItemViewModel()
        {
            ReleaseUnmanagedResources();
        }
    }
}