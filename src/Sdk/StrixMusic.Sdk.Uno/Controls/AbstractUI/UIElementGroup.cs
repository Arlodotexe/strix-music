using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.AbstractUI
{
	// <summary>
	/// A templated <see cref="Control"/> for display an <see cref="OwlCore.AbstractUI.AbstractTextBox"/>
	/// </summary>
	public sealed partial class UIElementGroup : Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UIElementGroup"/> class.
		/// </summary>
		public UIElementGroup()
		{
			this.DefaultStyleKey = typeof(UIElementGroup);
		}
	}
}
