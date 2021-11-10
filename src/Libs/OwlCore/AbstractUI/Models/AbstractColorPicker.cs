using System;
using System.Threading.Tasks;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a color picker. 
    /// </summary>
    public class AbstractColorPicker : AbstractUIElement
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractButton"/>.
        /// </summary>
        /// <param name="id"><inheritdoc cref="AbstractUIBase.Id"/></param>
        /// <param name="iconCode">The (optional) icon that is displayed with the label.</param>
        public AbstractColorPicker(string id, string? iconCode = null)
            : base(id)
        {
            IconCode = iconCode;
        }

        /// <summary>
        /// Called to notify listeners that a color has been picked.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        [RemoteMethod]
        public Task PickColor(string hex)
        {
            ColorPicked?.Invoke(this, hex);

            return Task.CompletedTask;
        }

        /// <summary>
        /// Raised when the user picks a color.
        /// </summary>
        public event EventHandler<string>? ColorPicked;
    }
}