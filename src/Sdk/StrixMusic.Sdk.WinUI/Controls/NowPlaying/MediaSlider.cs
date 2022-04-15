using System;
using System.Threading.Tasks;
using Windows.System.Threading;
using Windows.UI.Xaml;

namespace StrixMusic.Sdk.WinUI.Controls.NowPlaying
{
    /// <summary>
    /// A slider that can automatically update the tick position.
    /// </summary>
    public partial class MediaSlider : SliderEx
    {
        private ThreadPoolTimer _updateIntervalTimer;
        private DateTime _startTime = DateTime.Now;
        private bool _isManipulating;

        /// <summary>
        /// Initializes a new instance of the <see cref="MediaSlider"/> class.
        /// </summary>
        public MediaSlider()
        {
            DefaultStyleKey = typeof(MediaSlider);

            _updateIntervalTimer = ThreadPoolTimer.CreatePeriodicTimer(UpdateIntervalTimer_Tick, TimeSpan.FromMilliseconds(UpdateFrequency));

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
        /// The value of the slider. Contains a guard for manipulation events.
        /// </summary>
        public new double Value
        {
            get => base.Value;
            set 
            {
                if (!_isManipulating)
                    base.Value = value;
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
                ResumeTimer();
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
        }

        private void DetachEvents()
        {
            SliderManipulationStarted -= MediaSlider_SliderManipulationStarted;
            SliderManipulationCompleted -= MediaSlider_SliderManipulationCompleted;

            ValueChanged -= MediaSlider_ValueChanged;
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

        private async void UpdateIntervalTimer_Tick(ThreadPoolTimer timer)
        {
            await UpdateSliderValue();
        }

        private void MediaSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            _startTime = DateTime.Now - TimeSpan.FromMilliseconds(Value);
        }

        private void MediaSlider_SliderManipulationCompleted(object sender, EventArgs e)
        {
            _isManipulating = false;
            ResumeTimer();
        }

        private void MediaSlider_SliderManipulationStarted(object sender, EventArgs e)
        {
            _isManipulating = true;
            PauseTimer();
        }

        private void ResumeTimer()
        {
            _updateIntervalTimer?.Cancel();
            _startTime = DateTime.Now - TimeSpan.FromMilliseconds(Value);
            _updateIntervalTimer = ThreadPoolTimer.CreatePeriodicTimer(UpdateIntervalTimer_Tick, TimeSpan.FromMilliseconds(UpdateFrequency));
        }

        private void PauseTimer()
        {
            _updateIntervalTimer?.Cancel();
        }

        private async Task UpdateSliderValue()
        {
            await OwlCore.Threading.OnPrimaryThread(() => Value = (DateTime.Now - _startTime).TotalMilliseconds);
        }
    }
}
