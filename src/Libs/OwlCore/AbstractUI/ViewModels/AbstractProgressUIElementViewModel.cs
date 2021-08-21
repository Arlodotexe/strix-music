using OwlCore.AbstractUI.Models;
using System.ComponentModel;

namespace OwlCore.AbstractUI.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="AbstractProgressUIElement"/>.
    /// </summary>
    [Bindable(true)]
    public class AbstractProgressUIElementViewModel : AbstractUIViewModelBase
    {
        private bool _isIndeterminate;
        private double _value;
        private double _minimum;
        private double _maximum;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractProgressUIElementViewModel"/> class.
        /// </summary>
        /// <param name="model">The model to wrap around.</param>
        public AbstractProgressUIElementViewModel(AbstractProgressUIElement model)
            : base(model)
        {
            _value = model.Value ?? 0;
            _minimum = model.Minimum;
            _maximum = model.Maximum;
            _isIndeterminate = model.Value == null;

            AttachEvents(model);
        }

        private void AttachEvents(AbstractProgressUIElement model)
        {
            model.ValueChanged += Model_ValueChanged;
            model.MinimumChanged += Model_MinimumChanged;
            model.MaximumChanged += Model_MaximumChanged;
        }

        private void DetachEvents(AbstractProgressUIElement model)
        {
            model.ValueChanged -= Model_ValueChanged;
            model.MinimumChanged -= Model_MinimumChanged;
            model.MaximumChanged -= Model_MaximumChanged;
        }

        private void Model_ValueChanged(object sender, double? e)
        {
            _ = Threading.OnPrimaryThread(() =>
            {
                Value = e ?? 0;
                IsIndeterminate = e == null;
            });
        }

        private void Model_MinimumChanged(object sender, double e)
        {
            _ = Threading.OnPrimaryThread(() => Minimum = e);
        }

        private void Model_MaximumChanged(object sender, double e)
        {
            _ = Threading.OnPrimaryThread(() => Maximum = e);
        }

        /// <summary>
        /// Gets or sets the UI Element status minimum progress value.
        /// </summary>
        public double Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        /// <summary>
        /// Gets or sets the UI Element status minimum progress.
        /// </summary>
        public double Minimum
        {
            get => _minimum;
            set => SetProperty(ref _minimum, value);
        }

        /// <summary>
        /// Gets or sets the UI Element status maximum progress.
        /// </summary>
        public double Maximum
        {
            get => _maximum;
            set => SetProperty(ref _maximum, value);
        }

        /// <summary>
        /// Gets or sets if the UI Element status progress is indeterminate.
        /// </summary>
        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set => SetProperty(ref _isIndeterminate, value);
        }

        /// <inheritdoc/>
        public override void Dispose()
        {
            DetachEvents((AbstractProgressUIElement)Model);
            base.Dispose();
        }
    }
}
