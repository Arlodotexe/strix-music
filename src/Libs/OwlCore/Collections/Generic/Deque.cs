// Pulled from MoreCollections
// https://github.com/Avid29/MoreCollections

using System;
using System.Collections;
using System.Collections.Generic;

namespace OwlCore.Collections.Generic
{
    /// <summary>
    /// A strongly typed <see cref="Deque{T}"/> of objects.
    /// </summary>
    /// <typeparam name="T">The type of elements in the <see cref="Deque{T}"/>.</typeparam>
    public class Deque<T> : IEnumerable<T>
    {
        private const int _DefaultSize = 4;

        private T[] items;

        /// <summary>
        /// The index of the first item.
        /// </summary>
        private int frontIndex;

        /// <summary>
        /// The number of items in the <see cref="Deque{T}"/>.
        /// </summary>
        private int count;

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class.
        /// </summary>
        public Deque() : this(_DefaultSize)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class.
        /// </summary>
        /// <param name="size">The size of the Deque.</param>
        public Deque(int size)
        {
            items = new T[size];
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Deque{T}"/> class that contains elements
        /// copied from the specified collection.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <param name="chunkSize">Amount of memory reserved at a time <see cref="Deque{T}"/>.</param>
        public Deque(IEnumerable<T> collection, int chunkSize = _DefaultSize) : this(chunkSize)
        {
            foreach (var item in collection)
            {
                PushBack(item);
            }
        }

        /// <summary>
        /// Gets the number of items in the <see cref="Deque{T}"/>.
        /// </summary>
        public int Count => count;

        /// <summary>
        /// Gets the index of the last item in the <see cref="Deque{T}"/>.
        /// </summary>
        private int BackIndex => (frontIndex + count - 1) % items.Length;

        /// <summary>
        /// Gets or sets the value at <paramref name="index"/> in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="index">The index to get or set.</param>
        /// <returns>The value at <paramref name="index"/>.</returns>
        public T this[int index]
        {
            get
            {
                if (index > count)
                {
                    throw new IndexOutOfRangeException();
                }

                return items[(frontIndex + index) % items.Length];
            }

            set
            {
                if (index > count)
                {
                    throw new IndexOutOfRangeException();
                }

                items[(frontIndex + index) % items.Length] = value;
            }
        }

        /// <summary>
        /// Adds an object to the Front of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="value">The object to be added to the front of the <see cref="Deque{T}"/>.</param>
        public void PushFront(T value)
        {
            int newFront = (frontIndex - 1 + items.Length) % items.Length;
            if (items.Length == ++count)
            {
                Reallocate();
                newFront = items.Length - 1;
            }

            frontIndex = newFront;
            items[newFront] = value;
        }

        /// <summary>
        /// Adds an object to the back of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <param name="value">The object to be added to the back of the <see cref="Deque{T}"/>.</param>
        public void PushBack(T value)
        {
            int newBack = (BackIndex + 1) % items.Length;
            if (items.Length == ++count)
            {
                Reallocate();
                newBack = count - 1;
            }

            items[newBack] = value;
        }

        /// <summary>
        /// Removes and returns the object at the front of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The object that is removed from the front of the <see cref="Deque{T}"/>.</returns>
        public T PopFront()
        {
            T value = items[frontIndex];
            items[frontIndex] = default!;

            count--;
            frontIndex = (frontIndex + 1) % items.Length;
            return value;
        }

        /// <summary>
        /// Removes and returns the object at the back of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The object that is removed from the back of the <see cref="Deque{T}"/>.</returns>
        public T PopBack()
        {
            T value = this[count - 1];
            items[count - 1] = default!;

            count--;
            return value;
        }

        /// <summary>
        /// Gets the value from the front of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The frontmost value in the <see cref="Deque{T}"/>.</returns>
        public T PeekFront()
        {
            return items[frontIndex];
        }

        /// <summary>
        /// Gets the value from the back of the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>The backmost value in the <see cref="Deque{T}"/>.</returns>
        public T PeekBack()
        {
            return items[BackIndex];
        }

        /// <inheritdoc/>
        public IEnumerator<T> GetEnumerator()
        {
            return new DequeEnum<T>(this);
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Doubles allocation size and realligns the chunks.
        /// </summary>
        private void Reallocate()
        {
            T[] newItems = new T[items.Length * 2];

            // TODO: Try conditional front < back.
            int firstSegmentLength = items.Length - frontIndex;
            Array.Copy(items, frontIndex, newItems, 0, firstSegmentLength);
            Array.Copy(items, 0, newItems, firstSegmentLength, frontIndex);

            items = newItems;
            frontIndex = 0;
        }
    }

    /// <summary>
    /// Enumerates over each item in a <see cref="Deque{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of items in the <see cref="Deque{T}"/>.</typeparam>
    public struct DequeEnum<T> : IEnumerator<T>, IEnumerator
    {
        // Enumerators are positioned before the first element
        // until the first MoveNext() call.
        private int position; // -1 = not started, -2 = ended/disposed

        /// <summary>
        /// Initializes a new instance of the <see cref="DequeEnum{T}"/> class to enumerate over <paramref name="deque"/>.
        /// </summary>
        /// <param name="deque">The <see cref="Deque{T}"/> to enumerate over.</param>
        public DequeEnum(Deque<T> deque)
        {
            Deque = deque;
            position = -1;
        }

        /// <summary>
        /// Gets the IEnumerator <see cref="object"/> implimentation of <see cref="Current"/>.
        /// </summary>
        object IEnumerator.Current => Current!;

        /// <summary>
        /// Gets the current item in the <see cref="Deque{T}"/> from enumeration.
        /// </summary>
        public T Current
        {
            get
            {
                try
                {
                    return Deque[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }

        private Deque<T> Deque { get; }

        /// <summary>
        /// Moves to next item in the <see cref="Deque{T}"/>.
        /// </summary>
        /// <returns>Whether or not there are more items.</returns>
        public bool MoveNext()
        {
            position++;
            return position < Deque.Count;
        }

        /// <summary>
        /// Resets enumeration to the start of the <see cref="Deque{T}"/>.
        /// </summary>
        public void Reset()
        {
            position = -1;
        }

        /// <summary>
        /// Dereference and clean up.
        /// </summary>
        public void Dispose()
        {
            position = -2;
        }
    }
}