
// ReSharper disable once CheckNamespace

using System;
using System.Threading;
using Microsoft.Toolkit.Diagnostics;

namespace OwlCore
{
    /// <summary>
    /// Helpers related to Threading.
    /// </summary>
    public static partial class Threading
    {
        internal static SynchronizationContext? PrimarySyncContext { get; set; }

        /// <inheritdoc cref="DisposableSyncContext"/>
        public static DisposableSyncContext PrimaryContext => new DisposableSyncContext(PrimarySyncContext);

        /// <summary>
        /// A helper class that can run code wrapped in a <see langword="using"/> statement on the given sync context.
        /// </summary>
        public class DisposableSyncContext : IDisposable
        {
            private readonly SynchronizationContext _originalContext;

            /// <summary>
            /// Creates a new instance of <see cref="Threading.OnPrimaryThread"/>.
            /// </summary>
            public DisposableSyncContext(SynchronizationContext? newContext)
            {
                _originalContext = SynchronizationContext.Current;

                Guard.IsNotNull(newContext, nameof(newContext));

                SynchronizationContext.SetSynchronizationContext(newContext);
            }

            /// <inheritdoc />
            public void Dispose()
            {
                SynchronizationContext.SetSynchronizationContext(_originalContext);
            }
        }
    }
}