using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Toolkit.Diagnostics;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Extensions
{
    /// <summary>
    /// Provides helper methods for <see cref="ICore"/>s.
    /// </summary>
    public static partial class Cores
    {
        /// <summary>
        /// A syntactically sweet extension method to get a service from an instance of a core.
        /// </summary>
        /// <typeparam name="T">The type of service to find.</typeparam>
        /// <param name="sourceCore">The core instance to get the service from.</param>
        /// <returns>The service, if found, otherwise null.</returns>
        public static T? GetServiceSafe<T>(this ICore sourceCore)
            where T : class
        {
            if (sourceCore == null)
                throw new ArgumentNullException(nameof(sourceCore));

            return sourceCore.CoreConfig.Services?.GetService<T>();
        }

        /// <summary>
        /// A syntactically sweet extension method to get a service from an instance of a core.
        /// </summary>
        /// <typeparam name="T">The type of service to find.</typeparam>
        /// <param name="sourceCore">The core instance to get the service from.</param>
        /// <returns>The service, if found.</returns>
        /// <exception cref="InvalidOperationException"/>
        public static T GetService<T>(this ICore sourceCore)
            where T : class
        {
            if (sourceCore == null)
                throw new ArgumentNullException(nameof(sourceCore));

            Guard.IsNotNull(sourceCore.CoreConfig.Services, nameof(sourceCore.CoreConfig.Services));

            var service = sourceCore.CoreConfig.Services.GetService<T>();

            if (service is null)
                throw new InvalidOperationException();

            return service;
        }
    }
}
