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
            lock (_instanceCache)
            {
                if (_instanceCache.TryGetValue(id, out var value))
                    return value;

                var newItem = creationHandler();

                _instanceCache.Add(id, newItem);

                return newItem;
            }
        }

        /// <inheritdoc />
        public bool HasId(string id)
        {
            lock (_instanceCache)
            {
                return _instanceCache.TryGetValue(id, out var value);
            }
        }

        /// <inheritdoc />
        public void Replace(string id, T newValue)
        {
            lock (_instanceCache)
            {
                _instanceCache[id] = newValue;
            }
        }

        /// <inheritdoc />
        public T Remove(string id)
        {
            lock (_instanceCache)
            {
                var val = _instanceCache[id];
                _instanceCache.Remove(id);
                return val;
            }
        }
    }
}