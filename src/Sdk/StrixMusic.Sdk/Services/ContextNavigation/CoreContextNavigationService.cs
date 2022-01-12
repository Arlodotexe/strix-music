using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Services.ContextNavigation
{
    /// <inheritdoc />
    public sealed class CoreContextNavigationService : IContextNavigationService<ICoreMember?>
    {
        /// <inheritdoc />
        public event EventHandler<ContextNavigateEventArgs<ICoreMember?>>? NavigationRequested;

        /// <inheritdoc />
        public async Task RequestNavigation(string coreInstanceId, string contextId)
        {
            var sourceCore = MainViewModel.Singleton?.Cores.FirstOrDefault(c => c.InstanceId == coreInstanceId);

            Guard.IsNotNull(sourceCore, nameof(sourceCore));

            var coreMember = await sourceCore.GetContextById(contextId);

            var eventArgs = new ContextNavigateEventArgs<ICoreMember?>(coreMember);
            NavigationRequested?.Invoke(this, eventArgs);
        }
    }
}
