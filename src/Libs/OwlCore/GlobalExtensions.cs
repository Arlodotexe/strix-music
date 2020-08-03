using System;
using System.Threading.Tasks;
using OwlCore.ArchTools;
using OwlCore.Logger;

#pragma warning disable 649

namespace OwlCore
{
    /// <summary>
    /// "Global" helper extensions that are accessed using the <c>this</c> keyword
    /// </summary>
    public static class GlobalExtensions
    {
        private static LazyService<ILoggerService> _loggerService;

        /// <summary>
        /// Wraps code in a <c>try / catch</c> block, and handles any exceptions that are thrown.
        /// </summary>
        /// <param name="any">Can be any object</param>
        /// <param name="callback">The action to run in the <c>try / catch</c> block</param>
        /// <param name="logError"><see cref="bool"/> indicating if thrown exceptions should be logged</param>
        public static void TryOrSkip(this object any, Action callback, bool logError = true)
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                if (logError)
                    _ = Task.Run(() => _loggerService.Value.Log(ex));
            }
        }

        /// <summary>
        /// Wraps async code in a <c>try / catch</c> block, and handles any exceptions that are thrown.
        /// </summary>
        /// <param name="any">Can be any object</param>
        /// <param name="callback">The action to run in the <c>try / catch</c> block.</param>
        /// <param name="logError"><see cref="bool"/> indicating if thrown exceptions should be logged.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task TryOrSkipAsync(this object any, Func<Task<object>> callback, bool logError = true)
        {
            try
            {
                await callback();
            }
            catch (Exception ex)
            {
                if (logError)
                    await Task.Run(() => _loggerService.Value.Log(ex));
            }
        }
    }
}
