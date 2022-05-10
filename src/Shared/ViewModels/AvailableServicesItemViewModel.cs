using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Services.CoreManagement;

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
        /// <param name="coreManagementService">A core management service where cores can be re-registered.</param>
        /// <param name="metadata">The core metadata to wrap around.</param>
        public AvailableServicesItemViewModel(ICoreManagementService coreManagementService, CoreMetadata metadata)
        {
            _coreManagementService = coreManagementService;
            Metadata = metadata;

            CreateCoreInstanceCommand = new AsyncRelayCommand(CreateCoreInstanceAsync);
        }

        /// <inheritdoc cref="CoreMetadata"/>
        public CoreMetadata Metadata { get; }

        /// <summary>
        /// Fires when the user selects this item.
        /// </summary>
        public IAsyncRelayCommand CreateCoreInstanceCommand { get; }

        private Task CreateCoreInstanceAsync() => _coreManagementService.RegisterCoreInstanceAsync(Metadata);
    }
}
