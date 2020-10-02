using System;
using System.Collections.Generic;

namespace OwlCore.Services
{
    /// <summary>
    /// Assists in locating services registered with a certain object.
    /// </summary>
    public class ContextualServiceLocator
    {
        private readonly Dictionary<object, object> _serviceStore = new Dictionary<object, object>();

        /// <summary>
        /// Gets a service that was registered with a specific object.
        /// </summary>
        /// <typeparam name="T">The expected return type.</typeparam>
        /// <param name="context">The associated context.</param>
        /// <returns>The registered service.</returns>
        public T GetServiceByContext<T>(object context)
        {
            if (_serviceStore.TryGetValue(context, out var obj) && obj is T value)
                return value;

            throw new InvalidOperationException("Context isn't registered.");
        }

        /// <summary>
        /// Registers a new service and associates it with an object.
        /// </summary>
        /// <typeparam name="T">The return type for the service.</typeparam>
        /// <param name="service">The service to register.</param>
        /// <param name="context">The associated context.</param>
        public void Register<T>(T service, object context) where T : class
        {
            if (context is null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            _serviceStore.Add(context, service);
        }
    }
}
