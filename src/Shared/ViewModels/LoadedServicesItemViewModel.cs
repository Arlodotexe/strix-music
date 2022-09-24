using System;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Services.CoreManagement;

namespace StrixMusic.ViewModels
{
    /// <summary>
    /// Used to display an item in the service section of <see cref="SuperShell"/>.
    /// </summary>
    public class LoadedServicesItemViewModel : ObservableObject
    {
        private bool _canDeleteCore = true;

        /// <summary>
        /// Creates a new instance of <see cref="LoadedServicesItemViewModel"/>.
        /// </summary>
        /// <param name="isAddItem">Indicates that selecting this item will trigger adding a new core.</param>
        /// <param name="core">The core associated with this item, if applicable.</param>
        public LoadedServicesItemViewModel(bool isAddItem, CoreViewModel? core)
        {
            IsAddItem = isAddItem;
            Core = core;

            AddNewItemCommand = new RelayCommand(AddNewItem);
            DeleteCoreCommand = new AsyncRelayCommand(DeleteCore);
            ConfigureCoreCommand = new RelayCommand(ConfigureCore);
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
    }
}
