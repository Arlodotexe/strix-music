using System;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.Uno.Controls.NowPlaying
{
    /// <summary>
    /// A slider that can automatically update the tick position.
    /// </summary>
    public partial class MediaSlider : SliderEx
    {
        private readonly DispatcherTimer _updateIntervalTimer = new DispatcherTimer();
        private DateTime _startTime = DateTime.Now;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSlider"/> class.
        /// </summary>
        public MediaSlider()
        {
            DefaultStyleKey = typeof(MediaSlider);

            Loaded += OnLoaded;
        }

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="UpdateFrequency"/> property.
        /// </summary>
        public static readonly DependencyProperty UpdateFrequencyProperty =
            DependencyProperty.Register(
                nameof(UpdateFrequency),
                typeof(long),
                typeof(MediaSlider),
                new PropertyMetadata(50L)); // 100 milliseconds

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="UpdateFrequency"/> property.
        /// </summary>
        public static readonly DependencyProperty IsAdvancingProperty =
            DependencyProperty.Register(
                nameof(IsAdvancing),
                typeof(bool),
                typeof(MediaSlider),
                new PropertyMetadata(true));

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="TimeValue"/> property.
        /// </summary>
        public static readonly DependencyProperty TimeValueProperty =
            DependencyProperty.Register(
                nameof(TimeValue),
                typeof(TimeSpan),
                typeof(MediaSlider),
                new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the whether or not the slider is self advancing.
        /// </summary>
        public bool IsAdvancing
        {
            get => (bool)GetValue(IsAdvancingProperty);
            set
            {
                SetValue(IsAdvancingProperty, value);

                if (value)
                    ResumeTimer();
                else
                    PauseTimer();
            }
        }

        /// <summary>
        /// Gets or sets the frequency with which the slider should move forward.
        /// </summary>
        /// <remarks>
        /// Milliseconds
        /// </remarks>
        public long UpdateFrequency
        {
            get => (long)GetValue(UpdateFrequencyProperty);
            set
            {
                if (value <= 0)
                    throw new ArgumentOutOfRangeException(nameof(value), value, "UpdateFrequency must be greater than 0");

                SetValue(UpdateFrequencyProperty, value);
                UpdateTimer();
            }
        }

        /// <summary>
        /// The current value as a <see cref="TimeSpan"/>.
        /// </summary>
        public TimeSpan TimeValue
        {
            get => TimeSpan.FromMilliseconds(Value);
            set
            {
                Value = value.TotalMilliseconds;
                SetValue(UpdateFrequencyProperty, value);
            }
        }

        private void AttachEvents()
        {
            Unloaded += MediaSlider_Unloaded;

            // Pause the Timer while manipulating
            SliderManipulationStarted += MediaSlider_SliderManipulationStarted;
            SliderManipulationCompleted += MediaSlider_SliderManipulationCompleted;

            // Update the timer position when released
            ValueChanged += MediaSlider_ValueChanged;
            _updateIntervalTimer.Tick += UpdateIntervalTimer_Tick;
        }

        private void DetachEvents()
        {
            SliderManipulationStarted -= MediaSlider_SliderManipulationStarted;
            SliderManipulationCompleted -= MediaSlider_SliderManipulationCompleted;

            ValueChanged -= MediaSlider_ValueChanged;
            _updateIntervalTimer.Tick -= UpdateIntervalTimer_Tick;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            Loaded -= OnLoaded;
            AttachEvents();
        }

        private void MediaSlider_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachEvents();
        }

        private void UpdateIntervalTimer_Tick(object sender, object e)
        {
            UpdateSliderValue();
        }

        private void MediaSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _startTime = DateTime.Now - TimeSpan.FromMilliseconds(Value);
        }

        private void MediaSlider_SliderManipulationCompleted(object sender, EventArgs e)
        {
            ResumeTimer();
        }

        private void MediaSlider_SliderManipulationStarted(object sender, EventArgs e) => PauseTimer();

        private void UpdateTimer()
        {
            _updateIntervalTimer.Stop();
            _updateIntervalTimer.Interval = TimeSpan.FromMilliseconds(UpdateFrequency);
            ResumeTimer();
        }

        private void ResumeTimer()
        {
            _startTime = DateTime.Now - TimeSpan.FromMilliseconds(Value);
            _updateIntervalTimer.Start();
        }

        private void PauseTimer()
        {
            _updateIntervalTimer.Stop();
        }

        private void UpdateSliderValue()
        {
            Value = (DateTime.Now - _startTime).TotalMilliseconds;
        }
    }
}
