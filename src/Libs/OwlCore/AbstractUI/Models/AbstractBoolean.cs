using System;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a UI element that has a changeable boolean value. (Checkbox, Toggle Buttons, Switches, etc)
    /// </summary>
    public class AbstractBoolean : AbstractUIElement
    {
        private bool _state;
        private string _label;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractBoolean"/>/
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="label"><inheritdoc cref="Label"/></param>
        public AbstractBoolean(string id, string label)
            : base(id)
        {
            _label = label;
        }

        /// <summary>
        /// The label to display next to this UI element.
        /// </summary>
        [RemoteProperty]
        public string Label
        {
            get => _label;
            set
            {
                if (_label == value)
                    return;

                _label = value;

                LabelChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// The current state of this UI element.
        /// </summary>
        [RemoteProperty]
        public bool State
        {
            get => _state;
            set
            {
                if (_state == value)
                    return;

                _state = value;
                StateChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Fires when the <see cref="Label"/> changes.
        /// </summary>
        public event EventHandler<string>? LabelChanged;

        /// <summary>
        /// Fires when the <see cref="State"/> changes.
        /// </summary>
        public event EventHandler<bool>? StateChanged;
    }
}
