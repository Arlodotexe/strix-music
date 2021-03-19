using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using Microsoft.Toolkit.Mvvm.Input;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Shared.ViewModels
{
    /// <summary>
    /// A view model containing metadata about a service (core) that the user can create an instance of.
    /// </summary>
    public class AvailableServicesItemViewModel : ObservableObject
    {
        private readonly ICoreManagementService _coreManagementService;

        /// <summary>
        /// Initializes a new instance of the <see cref="AvailableServicesItemViewModel"/> class.
        /// </summary>
        /// <param name="assemblyInfo">The core assembly to wrap around.</param>
        public AvailableServicesItemViewModel(CoreAssemblyInfo assemblyInfo)
        {
            AssemblyInfo = assemblyInfo;

            _coreManagementService = Ioc.Default.GetRequiredService<ICoreManagementService>();

            CreateCoreInstanceCommand = new AsyncRelayCommand(CreateCoreInstance);
        }

        /// <inheritdoc cref="CoreAssemblyInfo"/>
        public CoreAssemblyInfo AssemblyInfo { get; }

        /// <summary>
        /// Fires when the user selects this item.
        /// </summary>
        public IAsyncRelayCommand CreateCoreInstanceCommand { get; }

        private async Task CreateCoreInstance()
        {
            Guard.IsNotNull(_coreManagementService, nameof(_coreManagementService));

            await _coreManagementService.RegisterCoreInstanceAsync(AssemblyInfo);
        }
    }
}
