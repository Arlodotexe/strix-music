using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OwlCore.AbstractUI.Models;
using LaunchPad.AbstractUI.ViewModels;
using Microsoft.Toolkit.Diagnostics;
using LaunchPad.AbstractUI.Controls;
using Windows.UI.Xaml.Data;

namespace LaunchPad.AbstractUI.Themes
{
    /// <summary>
    /// Selects the template that is used for an <see cref="AbstractButton"/> based on the <see cref="AbstractButton.ButtonType"/>.
    /// </summary>
    public class AbstractButtonTemplateSelector : IValueConverter
    {
        /// <summary>
        /// The data template used to a display an <see cref="AbstractButton"/> with a generic style.
        /// </summary>
        public Style? GenericStyle { get; set; }

        /// <summary>
        /// The data template used to a display an <see cref="AbstractButton"/> with a confirmation style.
        /// </summary>
        public Style? ConfirmStyle { get; set; }

        /// <summary>
        /// The data template used to a display an <see cref="AbstractButton"/> with a deletion style.
        /// </summary>
        public Style? DeleteStyle { get; set; }

        /// <inheritdoc />
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            AbstractButton.ButtonType type = AbstractButton.ButtonType.Generic;

            if (value is AbstractButton.ButtonType vType)
            {
                type = vType;
            }
            if (value is AbstractButtonViewModel buttonViewModel)
            {
                type = buttonViewModel.Type;
            }
            else if (value is AbstractButton button)
            {
                type = button.Type;
            }

            return type switch
            {
                AbstractButton.ButtonType.Generic => GenericStyle ?? ThrowHelper.ThrowArgumentNullException<Style>(),
                AbstractButton.ButtonType.Confirm => ConfirmStyle ?? GenericStyle ?? ThrowHelper.ThrowArgumentNullException<Style>(),
                AbstractButton.ButtonType.Delete => DeleteStyle ?? GenericStyle ?? ThrowHelper.ThrowArgumentNullException<Style>(),
                _ => throw new NotImplementedException(),
            };
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
