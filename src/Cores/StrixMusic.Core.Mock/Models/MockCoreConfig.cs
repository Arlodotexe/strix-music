using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Core.Mock.Services;
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
        public IServiceCollection Services { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MockCoreConfig"/> class.
        /// </summary>
        public MockCoreConfig(ICore sourceCore)
        {
            Services = new ServiceCollection();

            ConfigureServices(sourceCore.InstanceId);
        }


        /// <summary>
        /// Configures services for this instance of the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        private Task ConfigureServices(string instanceId)
        {
            Services.Add(new ServiceDescriptor(typeof(IMockCoreDataService), new JsonMockCoreDataService(instanceId)));
            Services.Add(new ServiceDescriptor(typeof(IMockCoreDataService), new MusicBrainzMockCoreDataService(instanceId)));

            return Task.CompletedTask;
        }
    }
}
