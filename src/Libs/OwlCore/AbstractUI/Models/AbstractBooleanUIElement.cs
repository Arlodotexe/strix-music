using System;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a UI element that has a changeable boolean value. (Checkbox, Toggle Buttons, Switches, etc)
    /// </summary>
    public class AbstractBooleanUIElement : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractBooleanUIElement"/>/
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="Label"/></param>
        public AbstractBooleanUIElement(string id, string label)
            : base(id)
        {
            Label = label;
        }

        /// <summary>
        /// The label to display next to this UI element.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// Fires when the <see cref="Label"/> changes.
        /// </summary>
        public event EventHandler<string>? LabelChanged;

        /// <summary>
        /// Called to change the <see cref="Label"/>.
        /// </summary>
        /// <param name="newValue">The new value for <see cref="Label"/>.</param>
        public void ChangeLabel(string newValue)
        {
            Label = newValue;
            LabelChanged?.Invoke(this, newValue);
        }

        /// <summary>
        /// The current state of this UI element.
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// Fires when the <see cref="State"/> changes.
        /// </summary>
        public event EventHandler<bool>? StateChanged;

        /// <summary>
        /// Called to change the <see cref="State"/>.
        /// </summary>
        public void ChangeState(bool newValue)
        {
            State = newValue;
            StateChanged?.Invoke(this, newValue);
        }
    }
}
