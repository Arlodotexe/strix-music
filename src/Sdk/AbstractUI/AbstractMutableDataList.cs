using System.Threading.Tasks;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// A <see cref="AbstractDataList"/> that can be changed by the user.
    /// </summary>
    public abstract class AbstractMutableDataList : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractMutableDataList"/>.
        /// </summary>
        /// <param name="id"></param>
        protected AbstractMutableDataList(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Called when the user wants to add a new item in the list. Behavior is defined by the core.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is the added item.</returns>
        public abstract Task<AbstractUIMetadata> AddItem();

        /// <summary>
        /// Fires when the user wants to remove an item from the <see cref="AbstractDataList.Items"/>.
        /// </summary>
        /// <param name="item">The item being removed</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task RemoveItem(AbstractUIMetadata item);
    }
}
