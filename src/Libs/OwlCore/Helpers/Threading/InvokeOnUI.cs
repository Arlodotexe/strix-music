using System;
using System.Threading;
using Microsoft.Toolkit.Diagnostics;

// ReSharper disable once CheckNamespace
namespace OwlCore.Helpers
{
    /// <summary>
    /// Helpers related to Threading.
    /// </summary>
    public static partial class Threading
    {
        /// <summary>
        /// Sets the <see cref="SynchronizationContext"/> for the UI thread.
        /// </summary>
        /// <param name="context">The UI thread.</param>
        public static void SetUISynchronizationContext(SynchronizationContext context)
        {
            if (!(UISyncContext is null))
                ThrowHelper.ThrowInvalidOperationException("Context cannot be set more than once.");

            UISyncContext = context;
        }

        /// <summary>
        /// Invokes a callback on the UI thread.
        /// </summary>
        /// <param name="callback">The callback to invoke.</param>
        public static void InvokeOnUI(Action callback)
        {
            InvokeOnUI(() => callback);
        }

        /// <summary>
        /// Invokes a callback on the UI thread.
        /// </summary>
        /// <typeparam name="T">The type to return.</typeparam>
        /// <param name="callback">The callback to invoke.</param>
        /// <returns>The value returned by the <paramref name="callback"/></returns>
        public static T InvokeOnUI<T>(Func<T> callback)
        {
            var originalContext = SynchronizationContext.Current;

            Guard.IsNotNull(UISyncContext, nameof(UISyncContext));

            SynchronizationContext.SetSynchronizationContext(UISyncContext);

            var returnValue = callback();

            SynchronizationContext.SetSynchronizationContext(originalContext);

            return returnValue;
        }
    }
}