using System.Collections.Generic;
using OwlCore.Remoting;
using OwlCore.Remoting.Attributes;
using System.Linq;
using System;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Presents a group of abstracted UI elements to the user.
    /// </summary>
    /// <remarks>
    /// Recommended to create a new <see cref="AbstractUIElementGroup"/> inside of <see cref="Items"/> for each section (Settings, About, etc).
    /// You can then create <see cref="AbstractUIElementGroup"/>s inside of each of these to group your settings, "About" data, etc.
    /// </remarks>
    [RemoteOptions(RemotingDirection.Bidirectional)]
    public class AbstractUIElementGroup : AbstractUIElement, IDisposable
    {
        private readonly MemberRemote _memberRemote;
        private readonly List<AbstractUIElement> _items = new List<AbstractUIElement>();

        /// <summary>
        /// Creates a new instance of an <see cref="AbstractUIElementGroup"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this element group.</param>
        /// <param name="preferredOrientation"></param>
        public AbstractUIElementGroup(string id, PreferredOrientation preferredOrientation = PreferredOrientation.Vertical)
            : base(id)
        {
            PreferredOrientation = preferredOrientation;
            _memberRemote = new MemberRemote(this, id);
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractUIElementGroup"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIElement this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this group.
        /// </summary>
        [RemoteProperty]
        public IReadOnlyList<AbstractUIElement> Items { get; set; } = new List<AbstractUIElement>();

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

        /// <inheritdoc cref="Models.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation { get; }

        /// <inheritdoc/>
        [RemoteMethod]
        public void Dispose()
        {
            _memberRemote.Dispose();
        }
    }
}
