using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Sdk.Services.Context
{
    /// <summary>
    /// The arguments for whenever navigation is requested with context.
    /// </summary>
    /// <typeparam name="T">The type of the navigation objects</typeparam>
    public class ContextNavigateEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextNavigateEventArgs{T}"/> class.
        /// </summary>
        /// <param name="page">The page to navigate to.</param>
        /// <param name="id">The context Id.</param>
        public ContextNavigateEventArgs(T page, string? id)
        {
            Page = page;
            Id = id;
        }

        /// <summary>
        /// Gets the page being navigated to.
        /// </summary>
        public T Page { get; }

        /// <summary>
        /// The context id.
        /// </summary>
        public string? Id { get; }
    }
}
