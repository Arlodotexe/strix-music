using System;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel wrapper for an <see cref="AbstractButton"/>.
    /// </summary>
    public class AbstractButtonViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractButton _model;

        /// <summary>
        /// Initializes a new instance of see <see cref="AbstractTextBoxViewModel"/>.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractButtonViewModel(AbstractButton model)
        : base(model)
        {
            _model = model;

            ClickCommand = new RelayCommand(ButtonClicked);
        }

        /// <summary>
        /// Text to show on the button.
        /// </summary>
        public AbstractButtonType Type
        {
            get => _model.Type;
            set => SetProperty(_model.Type, value, _model, (u, n) => _model.Type = n);
        }

        /// <summary>
        /// Text to show on the button.
        /// </summary>
        public string Text
        {
            get => _model.Text;
            set => SetProperty(_model.Text, value, _model, (u, n) => _model.Text = n);
        }

        /// <summary>
        /// Event that fires when the button is clicked.
        /// </summary>
        public event EventHandler Clicked
        {
            add => _model.Clicked += value;
            remove => _model.Clicked -= value;
        }

        /// <summary>
        /// Command for <see cref="AbstractButton"/> click.
        /// </summary>
        public IRelayCommand ClickCommand;

        /// <summary>
        /// Button clicked command.
        /// </summary>
        private void ButtonClicked()
        {
            _model.Click().Forget();
        }
    }
}
