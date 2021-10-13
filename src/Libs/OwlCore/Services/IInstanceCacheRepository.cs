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

        /// <summary>
        /// Gets an instance of an object.
        /// </summary>
        /// <param name="id">A unique identifier for this object.</param>
        /// <returns>An existing instance.</returns>
        bool HasId(string id);

        /// <summary>
        /// Overwrite an item that already exists in the cache.
        /// </summary>
        /// <param name="id">The ID of the item to replace.</param>
        /// <param name="newValue">The new value.</param>
        void Replace(string id, T newValue);

        /// <summary>
        /// Removes an item from the cache repo.
        /// </summary>
        /// <param name="id">The ID of the item to remove.</param>
        /// <returns>The removed item.</returns>
        public T Remove(string id);
    }
}