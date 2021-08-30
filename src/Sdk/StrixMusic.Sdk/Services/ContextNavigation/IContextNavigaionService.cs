using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Services.ContextNavigation
{
    /// <summary>
    /// A service that can be called, for example, by the app itself
    /// when activated via a protocol URL, or an App Service.
    /// <para>The data is processed and the current shell will be
    /// told to navigate to a page with the context.</para>
    /// </summary>
    public interface IContextNavigationService<T>
    {
        /// <summary>
        /// To request navigation for a target core.
        /// </summary>
        /// <param name="coreInstanceId">The name of the core.</param>
        /// <param name="contextId">The payload which can be a Uri or Id.</param>
        /// <returns>A <see cref="Task"/> that represents the asychronous operation.</returns>
        Task RequestNavigation(string coreInstanceId, string contextId);

        /// <summary>
        /// An event that is triggered whenever navigation is requested.
        /// </summary>
        event EventHandler<ContextNavigateEventArgs<T>>? NavigationRequested;
    }
}
