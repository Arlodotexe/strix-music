using System;
using System.Threading;

// ReSharper disable once CheckNamespace
namespace OwlCore.Helpers
{
    /// <summary>
    /// Helpers related to 
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
                throw new InvalidOperationException("Context cannot be set more than once.");

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

            if (UISyncContext is null)
                throw new InvalidOperationException($"UI context not found. {nameof(Threading.SetUISynchronizationContext)} was not called.");

            SynchronizationContext.SetSynchronizationContext(UISyncContext);

            var returnValue = callback();

            SynchronizationContext.SetSynchronizationContext(originalContext);

            return returnValue;
        }

        internal static SynchronizationContext? UISyncContext { get; set; }
    }
}