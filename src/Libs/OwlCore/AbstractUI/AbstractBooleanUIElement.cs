using System;
using System.Threading.Tasks;

namespace OwlCore.AbstractUI
{
    /// <summary>
    /// Represents a UI element that has a changeable boolean value. (Checkbox, Toggle Buttons, Switches, etc)
    /// </summary>
    public abstract class AbstractBooleanUIElement : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractBooleanUIElement"/>/
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="Label"/></param>
        protected AbstractBooleanUIElement(string id, string? label)
            : base(id)
        {
            Label = label;
        }

        /// <summary>
        /// The label to display next to this UI element.
        /// </summary>
        public string? Label { get; }

        /// <summary>
        /// Fires when the <see cref="Label"/> changes.
        /// </summary>
        public abstract event EventHandler<string>? LabelChanged;

        /// <summary>
        /// Called when the user changes the state.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public abstract Task ChangeState(bool newValue);
    }
}
