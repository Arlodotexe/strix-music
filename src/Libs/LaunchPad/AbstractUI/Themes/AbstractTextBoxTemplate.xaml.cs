using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.Themes
{
    /// <summary>
    /// Default template for the <see cref="AbstractTextBox"/>
    /// </summary>
    public sealed partial class AbstractTextBoxTemplate : ResourceDictionary
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractTextBoxTemplate"/> class.
        /// </summary>
        public AbstractTextBoxTemplate()
        {
            this.InitializeComponent();
        }
    }
}
