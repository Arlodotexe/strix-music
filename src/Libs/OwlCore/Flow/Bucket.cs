using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace OwlCore
{
    public static partial class Flow
    {
        /// <inheritdoc cref="Bucket{T}"/>
        /// <typeparam name="T">The type of item held in the collection.</typeparam>
        /// <param name="id">A unique identifier.</param>
        /// <param name="collection">The collection to wrap around.</param>
        /// <param name="timeToWait">A period of time to wait for items to stop being added to the bucket before emptying it.</param>
        /// <returns>A new <see cref="Bucket{T}"/>.</returns>
        public static Bucket<T> MakeBucket<T>(string id, ICollection<T> collection, TimeSpan timeToWait)
        {
            return new Bucket<T>(id, collection, timeToWait);
        }

        /// <inheritdoc cref="Bucket{T}"/>
        /// <typeparam name="T">The type of item held in the collection.</typeparam>
        /// <param name="id">A unique identifier.</param>
        /// <param name="collection">The collection to wrap around.</param>
        /// <param name="timeToWait">A period of time to wait for items to stop being added to the bucket before emptying it.</param>
        /// <param name="maxItems">The max number of items in the collection before the bucket empties.</param>
        /// <returns>A new <see cref="Bucket{T}"/>.</returns>
        public static Bucket<T> MakeBucket<T>(string id, ICollection<T> collection, TimeSpan timeToWait, int maxItems)
        {
            return new Bucket<T>(id, collection, timeToWait, maxItems);
        }
    }

    /// <summary>
    /// A bucket manages flow control after adding an item to a collection. The bucket empties when given conditions are met.
    /// </summary>
    /// <example>For when you have a ton of items being emitted from an event back to back, but want to work with batches of them elsewhere at all once.</example>
    /// <remarks>This does the job, but it could be improved 100 fold (inherit from ICollection?).</remarks>
    public sealed class Bucket<T>
    {
        private readonly ICollection<T> _collection;
        private readonly TimeSpan _timeToWait;
        private readonly int _maxItems;
        private readonly SemaphoreSlim _batchItemsLock = new SemaphoreSlim(1, 1);
        private readonly string _id;

        /// <summary>
        /// Creates a new instance of <see cref="Bucket{T}"/>.
        /// </summary>
        /// <param name="id">A unique identifier.</param>
        /// <param name="collection">The collection to wrap around.</param>
        /// <param name="timeToWait">A period of time to wait for items to stop being added to the bucket before emptying it.</param>
        internal Bucket(string id, ICollection<T> collection, TimeSpan timeToWait)
        {
            _id = id;
            _collection = collection;
            _timeToWait = timeToWait;
        }

        /// <summary>
        /// Creates a new instance of <see cref="Bucket{T}"/>.
        /// </summary>
        /// <param name="id">A unique identifier.</param>
        /// <param name="collection">The collection to wrap around.</param>
        /// <param name="timeToWait">A period of time to wait for items to stop being added to the bucket before emptying it.</param>
        /// <param name="maxItems">The max number of items in the collection before the bucket empties.</param>
        internal Bucket(string id, ICollection<T> collection, TimeSpan timeToWait, int maxItems)
        {
            _id = id;
            _collection = collection;
            _timeToWait = timeToWait;
            _maxItems = maxItems;
        }

        /// <summary>
        /// Adds an item to the bucket and attempt to satisfy emptying conditions.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public AwaitableDisposable<BucketState<T>> AddItemAsync(T item)
        {
            return AddItemAsync(item, CancellationToken.None);
        }

        /// <summary>
        /// Adds an item to the bucket and attempt to satisfy emptying conditions.
        /// </summary>
        /// <param name="item">The item to add.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that cancels the internal lock.</param>
        public AwaitableDisposable<BucketState<T>> AddItemAsync(T item, CancellationToken cancellationToken)
        {
            return new AwaitableDisposable<BucketState<T>>(RequestEntry(item, cancellationToken));
        }

        private async Task<BucketState<T>> RequestEntry(T item, CancellationToken cancellationToken)
        {
            await _batchItemsLock.WaitAsync(cancellationToken);
            _collection.Add(item);
            _batchItemsLock.Release();

            var debounced = await Flow.Debounce(_id, _timeToWait);
            var overMaxItems = _maxItems > 0 && _collection.Count < _maxItems;
            var willEmpty = debounced || overMaxItems;

            return new BucketState<T>(willEmpty, this);
        }

        internal void ReleaseLock()
        {
            _batchItemsLock.Release();
        }

        internal void EmptyCollection()
        {
            _collection.Clear();
        }
    }

    /// <summary>
    /// Holds state information for a bucket and provides disposable syntax.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class BucketState<T> : IDisposable
    {
        private readonly Bucket<T> _bucket;

        internal BucketState(bool willEmpty, Bucket<T> bucket)
        {
            _bucket = bucket;
            WillEmpty = willEmpty;
        }

        /// <summary>
        /// Indicates if the bucket has met the continue conditions.
        /// </summary>
        public bool WillEmpty { get; set; }

        /// <inheritdoc />
        public void Dispose()
        {
            _bucket.EmptyCollection();
            _bucket.ReleaseLock();
        }
    }
}