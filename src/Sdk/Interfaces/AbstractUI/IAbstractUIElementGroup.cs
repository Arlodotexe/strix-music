using System.Collections.Generic;

namespace StrixMusic.Sdk.Interfaces.AbstractUI
{
    /// <summary>
    /// Presents a group of abstracted UI elements to the user.
    /// </summary>
    /// <remarks>
    /// Recommended to create a new <see cref="IAbstractUIElementGroup"/> inside of <see cref="Items"/> for each section (Settings, About, etc).
    /// You can then create <see cref="IAbstractUIElementGroup"/>s inside of each of these to group your settings, "About" data, etc.
    /// </remarks>
    public interface IAbstractUIElementGroup : IAbstractUIElement
    {
        /// <summary>
        /// The items in this group.
        /// </summary>
        public IEnumerable<IAbstractUIElement> Items { get; }

        /// <inheritdoc cref="AbstractUI.PrefferedOrientation"/>
        public PrefferedOrientation PrefferedOrientation { get; }
    }
}
