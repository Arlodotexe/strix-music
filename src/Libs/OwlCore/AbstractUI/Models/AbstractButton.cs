using System;
using System.Threading.Tasks;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Presents a link to the user.
    /// </summary>
    /// <remarks>This can be displayed in the UI however it wants (Button, text link, Icons, custom, etc)</remarks>
    public class AbstractButton : AbstractUIMetadata
    {
        /// <summary>
        /// The label  that is displayed in the button.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractButton"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="text">The label that is displayed in the button.</param>
        /// <param name="iconCode">The (optional) icon that is displayed with the label.</param>
        public AbstractButton(string id, string text, string? iconCode = null)
            : base(id)
        {
            IconCode = iconCode;
            Text = text;
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
