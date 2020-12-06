﻿using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Helpers;

namespace LaunchPad.AbstractUI.ViewModels
{
    /// <summary>
    /// A ViewModel for the <see cref="AbstractTextBox"/>.
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

            ValueChangedCommand = new AsyncRelayCommand<TextChangedEventArgs>(HandleValueChanged);
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
            set
            {
                if (value == _model.Value)
                    return;

                _ = SaveValue(value);
                SetProperty(_model.Value, value, _model, (u, n) => _model.Value = n);
            }
        }

        /// <summary>
        /// Submits the value when the user has finished entering text.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        public async Task SaveValue(string newValue)
        {
            if (await Threading.Debounce(_id, TimeSpan.FromSeconds(1)))
            {
                ValueChanged?.Invoke(this, newValue);
                _model.SaveValue(newValue);
            }
        }

        private Task HandleValueChanged(TextChangedEventArgs args)
        {
            return SaveValue(Value);
        }

        /// <summary>
        /// Fires when <see cref="Value"/> is changed.
        /// </summary>
        public event EventHandler<string>? ValueChanged;

        /// <summary>
        /// Fire to notify that the value of the text box has been changed.
        /// </summary>
        public IRelayCommand<TextChangedEventArgs> ValueChangedCommand;
    }
}