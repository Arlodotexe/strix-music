using System;
using System.Collections.Generic;

namespace OwlCore.Services
{
    /// <summary>
    /// Handles creating a new or getting an existing instance of an object.
    /// </summary>
    public class InstanceCacheRepository<T> : IInstanceCacheRepository<T>
    {
        private readonly Dictionary<string, T> _instanceCache = new Dictionary<string, T>();

        /// <inheritdoc />
        public T GetOrCreate(string id, Func<T> creationHandler)
        {
            if (_instanceCache.TryGetValue(id, out var value))
                return value;

            var newItem = creationHandler();

            _instanceCache.Add(id, newItem);

            return newItem;
        }
    }
}