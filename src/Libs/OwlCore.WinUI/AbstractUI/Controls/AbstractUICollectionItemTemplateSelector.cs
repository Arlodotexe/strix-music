﻿using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using OwlCore.AbstractUI.ViewModels;
using CommunityToolkit.Diagnostics;
using OwlCore.AbstractUI.Models;

namespace OwlCore.WinUI.AbstractUI.Controls
{
    /// <summary>
    /// The template selector used to display Abstract UI elements. Use this to define your own custom styles for each control. You may specify the existing, default styles for those you don't want to override.
    /// </summary>
    public class AbstractUICollectionItemTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Creates a new instance of <see cref="AbstractUICollectionItemTemplateSelector"/>.
        /// </summary>
        public AbstractUICollectionItemTemplateSelector()
        {
            if (!new Themes.AbstractTextBoxStyle().TryGetValue("DefaultAbstractTextBoxTemplate", out var textBoxTemplate))
            {
                TextBoxTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(textBoxTemplate));
            }

            if (!new Themes.AbstractDataListStyle().TryGetValue("DefaultAbstractDataListTemplate", out var dataListTemplate))
            {
                DataListTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(dataListTemplate));
            }

            if (!new Themes.AbstractButtonStyle().TryGetValue("DefaultAbstractButtonTemplate", out var buttonTemplate))
            {
                ButtonTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(buttonTemplate));
            }

            if (!new Themes.AbstractMultiChoiceStyle().TryGetValue("DefaultAbstractMultipleChoiceTemplate", out var multiChoiceTemplate))
            {
                MultiChoiceTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(multiChoiceTemplate));
            }

            if (!new Themes.AbstractBooleanStyle().TryGetValue("DefaultAbstractBooleanTemplate", out var booleanTemplate))
            {
                BooleanTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(booleanTemplate));
            }

            if (!new Themes.AbstractProgressIndicatorStyle().TryGetValue("DefaultAbstractProgressIndicatorTemplate", out var progressTemplate))
            {
                ProgressTemplate = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(progressTemplate));
            }

            TextBoxTemplate = (DataTemplate)textBoxTemplate;
            DataListTemplate = (DataTemplate)dataListTemplate;
            ButtonTemplate = (DataTemplate)buttonTemplate;
            MultiChoiceTemplate = (DataTemplate)multiChoiceTemplate;
            BooleanTemplate = (DataTemplate)booleanTemplate;
            ProgressTemplate = (DataTemplate)progressTemplate;
        }

        /// <summary>
        /// The data template used to display an <see cref="AbstractUICollection"/>.
        /// </summary>
        public DataTemplate? ElementCollection { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractTextBox"/>.
        /// </summary>
        public DataTemplate TextBoxTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractDataList"/>.
        /// </summary>
        public DataTemplate DataListTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractButton"/>.
        /// </summary>
        public DataTemplate ButtonTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractBoolean"/>.
        /// </summary>
        public DataTemplate BooleanTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractProgressIndicator"/>.
        /// </summary>
        public DataTemplate ProgressTemplate { get; set; }

        /// <summary>
        /// The data template used to display an <see cref="AbstractMultiChoice"/>.
        /// </summary>
        public DataTemplate MultiChoiceTemplate { get; set; }

        /// <inheritdoc />
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            if (!new Themes.AbstractUICollectionPresenterStyle().TryGetValue("DefaultAbstractUICollectionTemplate", out var elementCollection))
                ElementCollection = ThrowHelper.ThrowArgumentNullException<DataTemplate>(nameof(elementCollection));

            ElementCollection = (DataTemplate)elementCollection;

            return item switch
            {
                AbstractTextBoxViewModel _ => TextBoxTemplate,
                AbstractDataListViewModel _ => DataListTemplate,
                AbstractButtonViewModel _ => ButtonTemplate,
                AbstractMultiChoiceViewModel _ => MultiChoiceTemplate,
                AbstractBooleanViewModel _ => BooleanTemplate,
                AbstractProgressIndicatorViewModel _ => ProgressTemplate,
                AbstractUICollectionViewModel _ => ElementCollection,
                _ => base.SelectTemplateCore(item, container)
            };
        }
    }
}