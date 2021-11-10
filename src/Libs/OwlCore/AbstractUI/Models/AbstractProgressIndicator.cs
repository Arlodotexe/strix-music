using System;
using OwlCore.Remoting;

namespace OwlCore.AbstractUI.Models
{
    /// <summary>
    /// Represents a UI element that displays a progress state. (ProgressBar, ProgressRing)
    /// </summary>
    public class AbstractProgressIndicator : AbstractUIElement
    {
        private double _minimum;
        private double _maximum;
        private double? _value;
        private bool _isIndeterminate;

        /// <summary>
        /// Creates a new instance of <see cref="AbstractProgressIndicator"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this UI element.</param>
        /// <param name="val">The value of the progess.</param>
        /// <param name="max">The maximum value of the progess.</param>
        /// <param name="min">The minimum value of the progess.</param>
        public AbstractProgressIndicator(string id, double? val, double max = 100, double min = 0)
            : base(id)
        {
            _value = val;
            _maximum = max;
            _minimum = min;
        }

        /// <summary>
        /// Creates a new instance of <see cref="AbstractProgressIndicator"/>.
        /// </summary>
        /// <param name="id">A unique identifier for this UI element.</param>
        /// <param name="isIndeterminate">If true, the progress value is indeterminate. <see cref="Value"/> must be null</param>
        public AbstractProgressIndicator(string id, bool isIndeterminate)
            : base(id)
        {
            IsIndeterminate = isIndeterminate;
        }

        /// <summary>
        /// Fires when the <see cref="Value"/> changes.
        /// </summary>
        public event EventHandler<double?>? ValueChanged;

        /// <summary>
        /// Fires when the <see cref="Maximum"/> changes.
        /// </summary>
        public event EventHandler<double>? MaximumChanged;

        /// <summary>
        /// Fires when the <see cref="Minimum"/> changes.
        /// </summary>
        public event EventHandler<double>? MinimumChanged;

        /// <summary>
        /// Fires when the <see cref="IsIndeterminate"/> changes.
        /// </summary>
        public event EventHandler<bool>? IsIndeterminateChanged;

        /// <summary>
        /// Gets or sets the value for the progress to be.
        /// </summary>
        /// <remarks>
        /// Value is null if indeterminate.
        /// </remarks>
        [RemoteProperty]
        public double? Value
        {
            get => _value;
            set
            {
                _value = value;
                ValueChanged?.Invoke(this, value);

                // Update IsIndeterminate only if it needs to be.
                if (value == null != IsIndeterminate)
                    IsIndeterminate = value == null;
            }
        }

        /// <summary>
        /// Gets or sets the maximum value for the progress to be.
        /// </summary>
        [RemoteProperty]
        public double Maximum
        {
            get => _maximum;
            set
            {
                _maximum = value;
                MaximumChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for the progress to be.
        /// </summary>
        [RemoteProperty]
        public double Minimum
        {
            get => _minimum;
            set
            {
                _minimum = value;
                MinimumChanged?.Invoke(this, value);
            }
        }

        /// <summary>
        /// Gets or sets the minimum value for the progress to be.
        /// </summary>
        [RemoteProperty]
        public bool IsIndeterminate
        {
            get => _isIndeterminate;
            set
            {
                _isIndeterminate = value;
                IsIndeterminateChanged?.Invoke(this, value);

                // Set Value to null only if IsIndeterminate is true. If false, retain current value.
                if (value != IsIndeterminate && value)
                    Value = null;
            }
        }
    }
}
