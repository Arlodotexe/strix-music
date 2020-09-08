using System.Collections.Generic;

namespace StrixMusic.Sdk.Interfaces.AbstractUI
{
    /// <summary>
    /// Presents a group of abstracted UI elements to the user.
    /// </summary>
    public interface IAbstractUIElementGroup : IAbstractUIMetadata
    {
        /// <summary>
        /// The items in this group.
        /// </summary>
        public IEnumerable<IAbstractUIMetadata> Items { get; }
    }
}
