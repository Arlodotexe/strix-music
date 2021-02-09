using System;
using System.Threading.Tasks;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Presents a link to the user.
    /// </summary>
    /// <remarks>This can be displayed in the UI however it wants (Button, text link, Icons, custom, etc)</remarks>
    public class AbstractButton : AbstractUIElement
    {
        /// <summary>
        /// The type of button to display.
        /// </summary>
        public enum ButtonType
        {
            /// <summary>
            /// A generic button, could serve any purpose.
            /// </summary>
            Generic,

            /// <summary>
            /// Some sort of confirmation button. Likely displayed with the accent color.
            /// </summary>
            Confirm,

            /// <summary>
            /// Scary click. Likely displayed in Red.
            /// </summary>
            Delete,
        }

        /// <summary>
        /// The label that is displayed in the button.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// The type of button.
        /// </summary>
        public ButtonType Type { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractButton"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="text">The label that is displayed in the button.</param>
        /// <param name="iconCode">The (optional) icon that is displayed with the label.</param>
        /// <param name="type">The type of button.</param>
        public AbstractButton(string id, string text, string? iconCode = null, ButtonType type = ButtonType.Generic)
            : base(id)
        {
            IconCode = iconCode;
            Text = text;
            Type = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Task Click()
        {
            Clicked?.Invoke(this, EventArgs.Empty);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Event that fires when the button is clicked.
        /// </summary>
        public event EventHandler? Clicked;
    }
}
