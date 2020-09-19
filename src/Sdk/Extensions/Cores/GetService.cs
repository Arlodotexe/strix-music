using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using StrixMusic.Sdk.Interfaces;

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
        /// <returns>The service, if found.</returns>
        public static T GetService<T>(this ICore sourceCore)
        {
            if (sourceCore == null)
                throw new ArgumentNullException(nameof(sourceCore));

            return sourceCore.CoreConfig.Services.GetService<T>();
        }
    }
}
