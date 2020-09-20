using System;
using System.Linq;

namespace StrixMusic.Sdk.Services.ContextNavigation
{
    /// <inheritdoc />
    public class ContextNavigationService<T> : IContextNavigationService<T>
    {
        /// <inheritdoc />
        public event EventHandler<ContextNavigateEventArgs<object?>>? NavigationRequested;

        /// <inheritdoc />
        public void RequestNavigation(string coreName, string payload)
        {
            var sourceCore = MainViewModel.Singleton?.LoadedCores.FirstOrDefault(c => c.Name == coreName);
            var playable = sourceCore?.GetIPlayableById(payload);
            var eventArgs = new ContextNavigateEventArgs<object?>(playable);
            NavigationRequested?.Invoke(this, eventArgs);
        }
    }
}
