using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Helpers;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel for the <see cref="AbstractTextBox"/>.
    /// </summary>
    public class AbstractTextBoxViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractTextBox _model;

        /// <summary>
        /// Initializes a new instance of see <see cref="AbstractTextBoxViewModel"/>.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractTextBoxViewModel(AbstractTextBox model)
        : base(model)
        {
            _model = model;
            ValueChangedCommand = new AsyncRelayCommand<string>(SaveValue);
        }

        /// <summary>
        /// Placeholder text to show when the text box is empty.
        /// </summary>
        public string PlaceholderText
        {
            get => _model.PlaceholderText;
            set => SetProperty(_model.PlaceholderText, value, _model, (u, n) => _model.PlaceholderText = n);
        }

        /// <summary>
        /// The initial or current value of the text box.
        /// </summary>
        public string Value
        {
            get => _model.Value;
            set => SetProperty(_model.Value, value, _model, (u, n) => _model.Value = n);
        }

        /// <summary>
        /// Called to tell the core about the new value.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        public async Task SaveValue(string newValue)
        {
            if (await Threading.Debounce(_model.Id, TimeSpan.FromMilliseconds(1000)))
            {
                Value = newValue;
                ValueChanged?.Invoke(this, newValue);
            }
        }

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public event EventHandler<string>? ValueChanged;

        /// <summary>
        /// Fire to notify that the value of the text box has been changed.
        /// </summary>
        public IRelayCommand<string> ValueChangedCommand;
    }
}