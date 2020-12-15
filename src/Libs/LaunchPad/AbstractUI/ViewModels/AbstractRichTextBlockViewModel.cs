using OwlCore.AbstractUI.Models;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml.Controls;

namespace LaunchPad.AbstractUI.ViewModels
{
    public class AbstractRichTextBlockViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractRichTextBlock _model;

        /// <summary>
        /// Creates a new instance of <see cref="RichTextBlock"/>.
        /// </summary>
        /// <param name="model"></param>
        public AbstractRichTextBlockViewModel(AbstractRichTextBlock model) : base(model)
        {
            _model = model;
        }

        /// <summary>
        /// Text to show when the <see cref="RichTextBlock"/>.
        /// </summary>
        public string Text
        {
            get => _model.RichText;
            set => SetProperty(_model.RichText, value, _model, (u, n) => _model.RichText = n);
        }
    }
}
