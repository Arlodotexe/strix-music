using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{

    /// <summary>
    /// A slider that can automatically update the tick position.
    /// </summary>
    public class ProgressSliderControl : Slider
    {
        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="UpdateFrequency"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateFrequencyProperty =
            DependencyProperty.Register(
                nameof(UpdateFrequency),
                typeof(int),
                typeof(ProgressSliderControl),
                new PropertyMetadata(100)); // 10 milliseconds

        private DispatcherTimer _updateIntervalTimer = new DispatcherTimer();

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressSliderControl"/> class.
        /// </summary>
        public ProgressSliderControl()
            : base()
        {
            UpdateTimer();
        }

        /// <summary>
        /// Gets or sets the frequency with which the slider should move forward.
        /// </summary>
        /// <remarks>
        /// ticks (100 nanoseconds)
        /// </remarks>
        public long UpdateFrequency
        {
            get => (long)GetValue(UpdateFrequencyProperty);
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, "UpdateFrequency must be greater than 0");
                }

                SetValue(UpdateFrequencyProperty, value);
                UpdateTimer();
            }
        }

        private void UpdateTimer()
        {
            _updateIntervalTimer.Stop();
            _updateIntervalTimer.Interval = new TimeSpan(UpdateFrequency);
            _updateIntervalTimer.Start();
        }

        private void UpdateSliderValue()
        {
            Value += UpdateFrequency;
        }
    }
}
