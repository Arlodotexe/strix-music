using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Sdk.Services.ContextNavigation
{
    /// <summary>
    /// A service that can be called, for example, by the app itself
    /// when activated via a protocol URL, or an App Service.
    /// The data would be processed and any loaded shell would be
    /// told to navigate to a page with the context.
    /// </summary>
    public interface IContextNavigationService<T>
    {
        /// <summary>
        /// To request navigation for a target core.
        /// </summary>
        /// <param name="coreName">The name of the core.</param>
        /// <param name="payload">The payload which can be a Uri or Id.</param>
        void RequestNavigation(string coreName, string payload);

        /// <summary>
        /// An event that is triggered whenever navigation is requested.
        /// </summary>
        event EventHandler<ContextNavigateEventArgs<object?>>? NavigationRequested;
    }
}
