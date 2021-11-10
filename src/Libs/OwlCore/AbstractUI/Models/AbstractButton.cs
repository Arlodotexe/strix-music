using System;
using System.Threading.Tasks;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a UI element that the user can click on to perform an action (Button, link, optional icon, etc)
    /// </summary>
    public class AbstractButton : AbstractUIElement
    {
        /// <summary>
        /// The label that is displayed in the button.
        /// </summary>
        [RemoteProperty]
        public string Text { get; set; }

        /// <summary>
        /// The type of button.
        /// </summary>
        [RemoteProperty]
        public AbstractButtonType Type { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractButton"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="text">The label that is displayed in the button.</param>
        /// <param name="iconCode">The (optional) icon that is displayed with the label.</param>
        /// <param name="type">The type of button.</param>
        public AbstractButton(string id, string text, string? iconCode = null, AbstractButtonType type = AbstractButtonType.Generic)
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
        [RemoteMethod]
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
