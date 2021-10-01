using System;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel wrapper for an <see cref="AbstractTextBox"/>.
    /// </summary>
    public class AbstractTextBoxViewModel : AbstractUIViewModelBase
    {
        private readonly AbstractTextBox _model;
        private readonly string _id;

        /// <summary>
        /// Initializes a new instance of see <see cref="AbstractTextBoxViewModel"/>.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractTextBoxViewModel(AbstractTextBox model)
        : base(model)
        {
            _model = model;
            _id = model.Id + Guid.NewGuid();

            ValueChangedCommand = new RelayCommand(HandleValueChanged);
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
        /// Submits the value when the user has finished entering text.
        /// </summary>
        public async Task SaveValue()
        {
            if (await Flow.Debounce(_id, TimeSpan.FromSeconds(2)))
            {
                _model.Value = Value;
            }
        }

        private void HandleValueChanged()
        {
            SaveValue().Forget();
        }

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public event EventHandler<string>? ValueChanged
        {
            add => _model.ValueChanged += value;
            remove => _model.ValueChanged -= value;
        }

        /// <summary>
        /// Fire to notify that the value of the text box has been changed.
        /// </summary>
        public IRelayCommand ValueChangedCommand;
    }
}