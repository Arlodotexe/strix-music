namespace OwlCore.Events
{
    /// <summary>
    /// Holds data about an item that was changed in a collection.
    /// </summary>
    /// <typeparam name="T">The type of the changed data.</typeparam>
    public class CollectionChangedEventArgsItem<T>
    {
        /// <summary>
        /// Creates a new instance of <see cref="CollectionChangedEventArgsItem{T}"/>.
        /// </summary>
        /// <param name="data"><inheritdoc cref="Data"/></param>
        /// <param name="index"><inheritdoc cref="Index"/></param>
        public CollectionChangedEventArgsItem(T data, int index)
        {
            Data = data;
            Index = index;
        }

        /// <summary>
        /// The item that was changed.
        /// </summary>
        public T Data { get; }

        /// <summary>
        /// The index of the changed item in the collection.
        /// </summary>
        public int Index { get; }
    }
}