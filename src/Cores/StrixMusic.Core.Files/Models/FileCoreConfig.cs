using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.Files.Services;
using StrixMusic.Sdk.Interfaces;
using StrixMusic.Sdk.Interfaces.AbstractUI;
using StrixMusic.Sdk.Services.Settings;

namespace StrixMusic.Core.Files
{
    /// <summary>
    /// Configures the <see cref="FileCore"/>.
    /// </summary>
    public class FileCoreConfig : ICoreConfig
    {
        /// <inheritdoc/>
        public IServiceProvider Services { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<IAbstractUIElementGroup> CoreDataUIElements => throw new NotImplementedException();

        /// <summary>
        /// Initializes a new instance of the <see cref="FileCoreConfig"/> class.
        /// </summary>
        public FileCoreConfig(ICore sourceCore)
        {
            Services = ConfigureServices(sourceCore.InstanceId);
        }

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        private ServiceProvider ConfigureServices(string instanceId)
        {
            IServiceCollection services = new ServiceCollection();

            services.Add(new ServiceDescriptor(typeof(ISettingsService), new FilesSettingsService(instanceId)));

            return services.BuildServiceProvider();
        }
    }
}
