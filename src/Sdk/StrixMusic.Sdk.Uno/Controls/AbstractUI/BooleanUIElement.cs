using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.AbstractUI
{
	/// <summary>
	/// A templated <see cref="Control"/> for display an <see cref="OwlCore.AbstractUI.AbstractBooleanUIElement"/>
	/// </summary>
	public sealed partial class BooleanUIElement : Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BooleanUIElement"/> class.
		/// </summary>
		public BooleanUIElement()
		{
			this.DefaultStyleKey = typeof(BooleanUIElement);
		}
	}
}
