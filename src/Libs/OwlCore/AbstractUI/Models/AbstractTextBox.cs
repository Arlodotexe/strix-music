using System;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Presents a text box to the user, with actions for saving any entered data.
    /// </summary>
    public class AbstractTextBox : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractTextBox"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        public AbstractTextBox(string id)
            : base(id)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractTextBox"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value">The initial value of the text box.</param>
        public AbstractTextBox(string id, string value)
            : base(id)
        {
            Value = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractTextBox"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value">The initial value of the text box.</param>
        /// <param name="placeholderText">Placeholder text to show when the text box is empty.</param>
        public AbstractTextBox(string id, string value, string placeholderText)
            : base(id)
        {
            Value = value;
            PlaceholderText = placeholderText;
        }

        /// <summary>
        /// Placeholder text to show when the text box is empty.
        /// </summary>
        public string PlaceholderText { get; set; } = string.Empty;

        /// <summary>
        /// The initial or current value of the text box.
        /// </summary>
        public string Value { get; set; } = string.Empty;

        /// <summary>
        /// Called to notify listeners about the new value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        public void SaveValue(string newValue)
        {
            Value = newValue;
            ValueChanged?.Invoke(this, newValue);
        }

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public event EventHandler<string>? ValueChanged;
    }
}
