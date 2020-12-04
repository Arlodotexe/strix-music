
// ReSharper disable once CheckNamespace

using System;
using System.Threading;
using Microsoft.Toolkit.Diagnostics;

namespace OwlCore.Helpers
{
    /// <summary>
    /// Helpers related to Threading.
    /// </summary>
    public static partial class Threading
    {
        internal static SynchronizationContext? UISyncContext { get; set; }

        /// <inheritdoc cref="UIThreadContext"/>
        public static UIThreadContext UIThread => new UIThreadContext();

        /// <summary>
        /// A helper class that can run code wrapped in a <see langword="using"/> statement on the UI thread.
        /// </summary>
        public class UIThreadContext : IDisposable
        {
            private readonly SynchronizationContext _originalContext;

            /// <summary>
            /// Creates a new instance of <see cref="InvokeOnUI"/>.
            /// </summary>
            public UIThreadContext()
            {
                _originalContext = SynchronizationContext.Current;

                Guard.IsNotNull(UISyncContext, nameof(UISyncContext));

                SynchronizationContext.SetSynchronizationContext(UISyncContext);
            }

            /// <inheritdoc />
            public void Dispose()
            {
                SynchronizationContext.SetSynchronizationContext(_originalContext);
            }
        }
    }
}