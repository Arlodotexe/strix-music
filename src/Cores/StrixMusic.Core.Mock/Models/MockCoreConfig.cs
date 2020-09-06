using System;
using Hqub.MusicBrainz.API;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.CoreInterfaces.Interfaces;
using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;

namespace StrixMusic.Core.Mock.Models
{
    /// <summary>
    /// MockCore config
    /// </summary>
    public class MockCoreConfig : ICoreConfig
    {
        /// <inheritdoc/>
        public IServiceProvider Services { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockCoreConfig"/> class.
        /// </summary>
        public MockCoreConfig(ICore sourceCore)
        {
            Services = ConfigureServices(sourceCore.InstanceId);
        }

        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        private IServiceProvider ConfigureServices(string instanceId)
        {
            IServiceCollection services = new ServiceCollection();

            services.Add(new ServiceDescriptor(typeof(MusicBrainzClient), new MusicBrainzClient()));

            return services.BuildServiceProvider();
        }
    }
}
