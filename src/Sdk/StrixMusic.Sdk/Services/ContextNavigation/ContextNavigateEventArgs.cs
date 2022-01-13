using System;

namespace StrixMusic.Sdk.Services.ContextNavigation
{
    /// <summary>
    /// The arguments for whenever navigation is requested with context.
    /// </summary>
    /// <typeparam name="T">The type of the navigation objects</typeparam>
    public sealed class ContextNavigateEventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContextNavigateEventArgs{T}"/> class.
        /// </summary>
        /// <param name="payload">An object to switch the context.</param>
        public ContextNavigateEventArgs(T payload)
        {
            Payload = payload;
        }

        /// <summary>
        /// An object against the context.
        /// </summary>
        public T Payload { get; }
    }
}
