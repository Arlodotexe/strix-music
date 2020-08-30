using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.Files.Services;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.Core.Files
{
    /// <summary>
    /// Configures the <see cref="FileCore"/>.
    /// </summary>
    public class FileCoreConfig : ICoreConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileCoreConfig"/> class.
        /// </summary>
        public FileCoreConfig(ICore sourceCore)
        {
            Services = new ServiceCollection();

            ConfigureServices(sourceCore.InstanceId);
        }

        /// <inheritdoc/>
        public IServiceCollection Services { get; }

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private Task ConfigureServices(string instanceId)
        {
            Services.Add(new ServiceDescriptor(typeof(FilesSettingsService), new FilesSettingsService(instanceId)));

            return Task.CompletedTask;
        }
    }
}
