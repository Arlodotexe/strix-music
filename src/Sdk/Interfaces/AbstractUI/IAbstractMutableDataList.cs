using System.Threading.Tasks;

namespace StrixMusic.Sdk.Interfaces.AbstractUI
{
    /// <summary>
    /// A <see cref="IAbstractDataList"/> that can be changed by the user.
    /// </summary>
    public interface IAbstractMutableDataList : IAbstractUIElement
    {
        /// <summary>
        /// Called when the user wants to add a new item in the list. Behavior is defined by the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the added item.</returns>
        public Task<IAbstractUIMetadata> AddItem();

        /// <summary>
        /// Fires when the user wants to remove an item from the <see cref="IAbstractDataList.Items"/>.
        /// </summary>
        /// <param name="item">The item being removed</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task RemoveItem(IAbstractUIMetadata item);
    }
}
