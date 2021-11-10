using System;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a box where the user can enter text.
    /// </summary>
    public class AbstractTextBox : AbstractUIElement
    {
        private string _value = string.Empty;
        private string _placeholderText = string.Empty;

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
            : this(id)
        {
            _value = value;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractTextBox"/>.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="value">The initial value of the text box.</param>
        /// <param name="placeholderText">Placeholder text to show when the text box is empty.</param>
        public AbstractTextBox(string id, string value, string placeholderText)
            : this(id, value)
        {
            PlaceholderText = placeholderText;
        }

        /// <summary>
        /// Placeholder text to show when the text box is empty.
        /// </summary>
        [RemoteProperty]
        public string PlaceholderText
        {
            get => _placeholderText;
            set
            {
                _placeholderText = value;
                PlaceholderTextChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// The initial or current value of the text box.
        /// </summary>
        [RemoteProperty]
        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public event EventHandler<string>? ValueChanged;

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public event EventHandler<string>? PlaceholderTextChanged;
    }
}
