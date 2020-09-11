using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a text box to the user, with actions for saving any entered data.
    /// </summary>
    public interface IAbstractTextBox : IAbstractUIElement
    {
        /// <summary>
        /// Placeholder text to show when the text box is empty.
        /// </summary>
        public string PlaceholderText { get; }

        /// <summary>
        /// The initial or current value of the text box.
        /// </summary>
        public string Value { get; }

        /// <summary>
        /// Called to tell the core about the new value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task SaveValue(string newValue);

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public event EventHandler<string> ValueChanged;
    }
}
