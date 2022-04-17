using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Sdk.CoreManagement;
using StrixMusic.Sdk.CoreModels;

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
        /// <param name="metadata">The core metadata to wrap around.</param>
        public AvailableServicesItemViewModel(CoreMetadata metadata)
        {
            Metadata = metadata;

            _coreManagementService = Ioc.Default.GetRequiredService<ICoreManagementService>();

            CreateCoreInstanceCommand = new AsyncRelayCommand(CreateCoreInstance);
        }

        /// <inheritdoc cref="CoreMetadata"/>
        public CoreMetadata Metadata { get; }

        /// <summary>
        /// Fires when the user selects this item.
        /// </summary>
        public IAsyncRelayCommand CreateCoreInstanceCommand { get; }

        private async Task CreateCoreInstance()
        {
            Guard.IsNotNull(_coreManagementService, nameof(_coreManagementService));

            await _coreManagementService.RegisterCoreInstanceAsync(Metadata);
        }
    }
}
