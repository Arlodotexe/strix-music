using System;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.AbstractUI
{
    /// <summary>
    /// Represents a UI element that has a changeable boolean value. (Checkbox, Toggle Buttons, Switches, etc)
    /// </summary>
    public interface IAbstractBooleanUIElement : IAbstractUIElement
    {
        /// <summary>
        /// The label to display next to this UI element.
        /// </summary>
        public string? Label { get; }

        /// <summary>
        /// Fires when the <see cref="Label"/> changes.
        /// </summary>
        public event EventHandler<string> LabelChanged;

        /// <summary>
        /// Called when the user changes the state.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public Task ChangeState(bool newValue);
    }
}
