using System;

namespace OwlCore.Services
{
    /// <summary>
    /// Handles creating a new or getting an existing instance of an object.
    /// </summary>
    public interface IInstanceCacheRepository<T>
    {
        /// <summary>
        /// Get or create an instance of an object.
        /// </summary>
        /// <param name="id">A unique identifier for this object.</param>
        /// <param name="creationHandler">A callback that creates</param>
        /// <returns>A new or existing instance.</returns>
        T GetOrCreate(string id, Func<T> creationHandler);
    }
}