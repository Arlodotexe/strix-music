using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OwlCore.Extensions;
using OwlCore.Remoting;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// A special <see cref="ICollection"/> that holds <see cref="AbstractUIElement"/>s, with additional options for presenting them.
    /// </summary>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public class AbstractUICollection : AbstractUIElement, ICollection<AbstractUIElement>
    {
        private List<AbstractUIElement> _items;

        /// <summary>
        /// Creates a new instance of an <see cref="AbstractUICollection"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this element group.</param>
        /// <param name="preferredOrientation"></param>
        public AbstractUICollection(string id, PreferredOrientation preferredOrientation = PreferredOrientation.Vertical)
            : base(id)
        {
            PreferredOrientation = preferredOrientation;
            _items = new List<AbstractUIElement>();
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractUICollection"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIElement this[int i] => Items.ElementAt(i);

        /// <inheritdoc/>
        public int Count => ((ICollection<AbstractUIElement>)_items).Count;

        /// <inheritdoc/>
        public bool IsReadOnly => ((ICollection<AbstractUIElement>)_items).IsReadOnly;

        /// <inheritdoc cref="Models.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation { get; }

        /// <summary>
        /// The items in this group.
        /// </summary>
        [RemoteProperty]
        public IReadOnlyList<AbstractUIElement> Items
        {
            get => _items;
            set => _items = value.ToOrAsList();
        }

        /// <summary>
        /// Adds the given <paramref name="abstractUIElement"/> to <see cref="Items" />.
        /// </summary>
        /// <param name="abstractUIElement">The item to add.</param>
        [RemoteMethod]
        public void Add(AbstractUIElement abstractUIElement) => _items.Add(abstractUIElement);

        /// <summary>
        /// Removes the given <paramref name="abstractUIElement"/> from <see cref="Items" />.
        /// </summary>
        /// <param name="abstractUIElement">The item to remove.</param>
        [RemoteMethod]
        public void Remove(AbstractUIElement abstractUIElement) => _items.Remove(abstractUIElement);

        /// <inheritdoc/>
        [RemoteMethod]
        public void Clear()
        {
            ((ICollection<AbstractUIElement>)_items).Clear();
        }

        /// <inheritdoc/>
        public bool Contains(AbstractUIElement item)
        {
            return ((ICollection<AbstractUIElement>)_items).Contains(item);
        }

        /// <inheritdoc/>
        [RemoteMethod]
        public void CopyTo(AbstractUIElement[] array, int arrayIndex)
        {
            ((ICollection<AbstractUIElement>)_items).CopyTo(array, arrayIndex);
        }

        /// <inheritdoc/>
        [RemoteMethod]
        bool ICollection<AbstractUIElement>.Remove(AbstractUIElement item)
        {
            return ((ICollection<AbstractUIElement>)_items).Remove(item);
        }

        /// <inheritdoc/>
        public IEnumerator<AbstractUIElement> GetEnumerator()
        {
            return ((IEnumerable<AbstractUIElement>)_items).GetEnumerator();
        }

        /// <inheritdoc/>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
    }
}
