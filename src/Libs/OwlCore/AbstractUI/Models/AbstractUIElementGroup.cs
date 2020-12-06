using System.Collections.Generic;
using System.Linq;

namespace OwlCore.AbstractUI.Models
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
            PreferredOrientation = preferredOrientation;
        }

        /// <summary>
        /// Get an item from this <see cref="AbstractUIElementGroup"/>.
        /// </summary>
        /// <param name="i">The index</param>
        public AbstractUIElement this[int i] => Items.ElementAt(i);

        /// <summary>
        /// The items in this group.
        /// </summary>
        public List<AbstractUIElement> Items { get; set; } = new List<AbstractUIElement>();

        /// <inheritdoc cref="Models.PreferredOrientation"/>
        public PreferredOrientation PreferredOrientation { get; }
    }
}
