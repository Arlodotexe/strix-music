using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.AbstractUI
{
	/// <summary>
	/// A templated <see cref="Control"/> for display an <see cref="OwlCore.AbstractUI.AbstractRichTextBlock"/>
	/// </summary>
	public sealed partial class RichTextBlockUIElement : Control
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="RichTextBlockUIElement"/> class.
		/// </summary>
		public RichTextBlockUIElement()
		{
			this.DefaultStyleKey = typeof(RichTextBlockUIElement);
		}
	}
}
