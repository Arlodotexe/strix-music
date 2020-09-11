using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Presents a text box to the user, with actions for saving any entered data.
    /// </summary>
    public abstract class AbstractTextBox : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractTextBox"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        protected AbstractTextBox(string id)
            : base(id)
        {
            Value = string.Empty;
            PlaceholderText = string.Empty;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractTextBox"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value"><inheritdoc cref="Value"/></param>
        /// <param name="placeholderText"><inheritdoc cref="PlaceholderText"/></param>
        protected AbstractTextBox(string id, string value, string placeholderText)
            : base(id)
        {
            Value = value;
            PlaceholderText = placeholderText;
        }

        /// <summary>
        /// Placeholder text to show when the text box is empty.
        /// </summary>
        public string PlaceholderText { get; protected set; }

        /// <summary>
        /// The initial or current value of the text box.
        /// </summary>
        public string Value { get; protected set; }

        /// <summary>
        /// Called to tell the core about the new value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task SaveValue(string newValue);

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public abstract event EventHandler<string> ValueChanged;
    }
}
