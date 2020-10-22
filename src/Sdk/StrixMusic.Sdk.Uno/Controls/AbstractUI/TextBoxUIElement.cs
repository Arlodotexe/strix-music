using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.AbstractUI
{
	// <summary>
	/// A templated <see cref="Control"/> for display an <see cref="OwlCore.AbstractUI.AbstractTextBox"/>
	/// </summary>
	public sealed partial class TextBoxUIElement : Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextBoxUIElement"/> class.
		/// </summary>
		public TextBoxUIElement()
		{
			this.DefaultStyleKey = typeof(TextBoxUIElement);
		}
	}
}
