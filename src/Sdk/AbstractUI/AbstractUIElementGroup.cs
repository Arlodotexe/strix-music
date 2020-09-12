using System;
using System.Collections.Generic;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a group of abstracted UI elements to the user.
    /// </summary>
    /// <remarks>
    /// Recommended to create a new <see cref="AbstractUIElementGroup"/> inside of <see cref="Items"/> for each section (Settings, About, etc).
    /// You can then create <see cref="AbstractUIElementGroup"/>s inside of each of these to group your settings, "About" data, etc.
    /// </remarks>
    public class AbstractUIElementGroup : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of an <see cref="AbstractUIElementGroup"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="preferredOrientation"></param>
        public AbstractUIElementGroup(string id, PreferredOrientation preferredOrientation)
            : base(id)
        {
            Items = Array.Empty<AbstractUIElement>();
            PreferredOrientation = preferredOrientation;
        }

        /// <summary>
        /// The items in this group.
        /// </summary>
        public IEnumerable<AbstractUIElement> Items { get; protected set; }

        /// <inheritdoc cref="AbstractUI.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation { get; }
    }
}
