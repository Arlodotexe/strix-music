using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// Abstract button viewmodel.
    /// </summary>
    public class AbstractButtonViewModel:AbstractUIViewModelBase
    {
        private readonly AbstractButton _model;
        private readonly string _id;

        /// <summary>
        /// Initializes a new instance of see <see cref="AbstractTextBoxViewModel"/>.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractButtonViewModel(AbstractButton model)
        : base(model)
        {
            _model = model;
            _id = model.Id + Guid.NewGuid();
            ClickCommand = new RelayCommand<EventArgs>(ButtonClicked);
        }
     
        /// <summary>
        /// Text to show when the button.
        /// </summary>
        public string Text
        {
            get => _model.Text;
            set => SetProperty(_model.Text, value, _model, (u, n) => _model.Text = n);
        }

        /// <summary>
        /// Event that fires when the button is clicked.
        /// </summary>
        public event EventHandler? Clicked;

        /// <summary>
        /// Command for <see cref="AbstractButton"/> click.
        /// </summary>
        public IRelayCommand<EventArgs> ClickCommand;

        /// <summary>
        /// Button clicked command.
        /// </summary>
        /// <param name="e"></param>
        private void ButtonClicked(EventArgs e)
        {
            Clicked?.Invoke(this, e);
        }

    }
}
