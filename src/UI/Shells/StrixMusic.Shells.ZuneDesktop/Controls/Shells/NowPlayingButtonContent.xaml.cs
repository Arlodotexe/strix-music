using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Shells
{
    /// <summary>
    /// A Control to display bars dancing as if music were playing.
    /// </summary>
    public sealed partial class NowPlayingButtonContent : UserControl
    {
        private const double PADDING = .8;
        private const double MINIMUM_HEIGHT_FACTOR = .05;
        private const double MINIMUM_CHANGE_FACTOR = 0.25;
        private const double POST_NOISE = 0.05;
        private const double SECONDS = .8;
        private Rectangle[] _rectangles;
        private double[] _activeScale;
        private ScaleTransform[] _rectScaleTransforms;
        private DoubleAnimation[] _dAnimations;
        private Storyboard _storyboard;
        private Random _random;

        /// <summary>
        /// <see cref="DependencyProperty"/> for the <see cref="BarCount"/> property.
        /// </summary>
        public static readonly DependencyProperty BarCountProperty =
            DependencyProperty.Register(
                nameof(BarCount),
                typeof(int),
                typeof(NowPlayingButtonContent),
                new PropertyMetadata(4));

        /// <summary>
        /// Initializes a new instance of the <see cref="NowPlayingButtonContent"/> class.
        /// </summary>
        public NowPlayingButtonContent()
        {
            this.InitializeComponent();
            _activeScale = new double[0];
            _rectangles = new Rectangle[0];
            _rectScaleTransforms = new ScaleTransform[0];
            _dAnimations = new DoubleAnimation[0];
            _storyboard = new Storyboard();
            _random = new Random();

            Loaded += NowPlayingButtonContent_Loaded;
        }

        private void NowPlayingButtonContent_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= NowPlayingButtonContent_Loaded;
            Unloaded += NowPlayingButtonContent_Unloaded;

            CreateLayout();
            CreateAndBeginStoryboard();
        }

        private void NowPlayingButtonContent_Unloaded(object sender, RoutedEventArgs e)
        {
            Unloaded -= NowPlayingButtonContent_Unloaded;
        }

        /// <summary>
        /// Gets or sets how many bars to display.
        /// </summary>
        public int BarCount
        {
            get => (int)GetValue(BarCountProperty);
            set
            {
                SetValue(BarCountProperty, value);
                CreateLayout();
            }
        }

        private void CreateLayout()
        {
            ColumnDefinitionCollection columns = RootGrid.ColumnDefinitions;
            columns.Clear();
            _rectangles = new Rectangle[BarCount];
            _rectScaleTransforms = new ScaleTransform[BarCount];

            // Calculate the width of all rectangles to display
            double rectWidth = (RootGrid.ActualWidth / BarCount) - (PADDING * (BarCount - 1));

            for (int i = 0; i < BarCount; i++)
            {
                columns.Add(new ColumnDefinition());

                Rectangle rect = CreateRectangle(rectWidth);
                ScaleTransform scaleTransform = SetRectangleRenderTransform(rect);
                scaleTransform.ScaleY = _random.NextDouble();
                Grid.SetColumn(rect, i);

                _rectangles[i] = rect;
                _rectScaleTransforms[i] = scaleTransform;
                RootGrid.Children.Add(rect);
            }
        }

        private void CreateAndBeginStoryboard()
        {
            _storyboard = new Storyboard();
            TimelineCollection timelines = _storyboard.Children;
            _dAnimations = new DoubleAnimation[_rectangles.Length];
            _activeScale = new double[_rectangles.Length];
            for (int i = 0; i < _rectScaleTransforms.Length; i++)
            {
                _activeScale[i] = _random.NextDouble();

                DoubleAnimation dAnimation = new DoubleAnimation();
                Storyboard.SetTarget(dAnimation, _rectScaleTransforms[i]);
                Storyboard.SetTargetProperty(dAnimation, "ScaleY");
                _dAnimations[i] = dAnimation;
                timelines.Add(dAnimation);
            }

            _storyboard.Duration = new Duration(TimeSpan.FromSeconds(SECONDS));
            _storyboard.Completed += Storyboard_Completed;
            _storyboard.Begin();
        }

        private Rectangle CreateRectangle(double width)
        {
            Rectangle rect = new Rectangle();

            rect.Fill = new SolidColorBrush(Windows.UI.Color.FromArgb(255, 219, 13, 148));
            rect.Width = width;

            return rect;
        }

        private ScaleTransform SetRectangleRenderTransform(Rectangle rect)
        {
            ScaleTransform scaleTransform = new ScaleTransform();

            rect.RenderTransformOrigin = new Windows.Foundation.Point(.5, 1);
            rect.RenderTransform = scaleTransform;

            return scaleTransform;
        }

        private void Storyboard_Completed(object sender, object e)
        {
            for (int i = 0; i < _dAnimations.Length; i++)
            {
                double value = _random.NextDouble();
                double active = _activeScale[i];
                double invalidMin = active - MINIMUM_CHANGE_FACTOR;
                double invalidMax = active + MINIMUM_CHANGE_FACTOR;
                invalidMin = Math.Max(invalidMin, MINIMUM_HEIGHT_FACTOR);
                invalidMax = Math.Min(invalidMax, 1);
                double invalidRange = invalidMax - invalidMin;
                double invalidRangeFull = invalidRange + MINIMUM_HEIGHT_FACTOR;

                value *= 1 - invalidRangeFull;
                value += MINIMUM_HEIGHT_FACTOR;

                if (value > invalidMin)
                {
                    value += invalidRange;
                }

                double noise = _random.NextDouble();
                noise *= POST_NOISE * 2;
                noise -= POST_NOISE;
                value += noise;

                _activeScale[i] = value;
                _dAnimations[i].To = value;
            }

            _storyboard.Begin();
        }
    }
}
