using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;
using StrixMusic.Core.LocalFiles;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace StrixMusic.Cores.OneDrive
{
    ///<inheritdoc/>
    public class OneDriveCore : LocalFilesCore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public OneDriveCore(string instanceId)
            : base(instanceId)
        {
            CoreConfig = new OneDriveCoreConfig(this);
        }


        /// <inheritdoc/>
        public async override Task InitAsync(IServiceCollection services)
        {
            ChangeCoreState(Sdk.Data.CoreState.Loading);

            if (!(CoreConfig is LocalFilesCoreConfig coreConfig))
                return;

            await coreConfig.SetupConfigurationServices(services);

            ChangeCoreState(Sdk.Data.CoreState.NeedsSetup);
        }
    }
}
