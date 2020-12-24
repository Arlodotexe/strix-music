using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using LaunchPad.AbstractUI.ViewModels;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractUI.Models;

namespace LaunchPad.AbstractUI.Themes
{
    /// <summary>
    /// Selects the template that is used for an <see cref="AbstractDataList"/> based on the <see cref="AbstractDataList.PreferredDisplayMode"/>.
    /// </summary>
    public class AbstractMultiChoiceTypeTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// The data template used to display an <see cref="AbstractTextBox"/>.
        /// </summary>
        public DataTemplate? ComboBoxTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractDataList"/>.
        /// </summary>
        public DataTemplate? RadioButtonTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (item is AbstractMultiChoiceUIElementViewModel viewModel)
            {
                return viewModel.PreferredDisplayMode switch
                {
                    AbstractMultiChoicePreferredDisplayMode.Dropdown => ComboBoxTemplate ?? ThrowHelper.ThrowArgumentNullException<DataTemplate>(),
                    AbstractMultiChoicePreferredDisplayMode.RadioButtons => RadioButtonTemplate ?? ThrowHelper.ThrowArgumentNullException<DataTemplate>(),
                    _ => throw new NotImplementedException(),
                };
            }

            return base.SelectTemplateCore(item, container);
        }
    }
}