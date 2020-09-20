using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Services.Context
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
        /// The target core.
        /// </summary>
        ICore TargetCore { get; }

        /// <summary>
        /// Id of the context.
        /// </summary>
        string? Id { get; }

        /// <summary>
        ///  Context Uri.
        /// </summary>
        Uri? Uri { get; }

        /// <summary>
        /// An event that is triggered whenever navigation is requested.
        /// </summary>
        event EventHandler<ContextNavigateEventArgs<T>> NavigationRequested;
    }
}
