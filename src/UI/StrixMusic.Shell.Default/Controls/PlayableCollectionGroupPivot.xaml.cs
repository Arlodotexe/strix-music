using StrixMusic.Sdk.Observables;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Shell.Default.Controls
{
    /// <summary>
    /// Displays the content of a ObservableCollectionGroup in a Pivot.
    /// </summary>
    public sealed partial class PlayableCollectionGroupPivot : UserControl
    {
        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="RestoreSelectedPivot"/> property.
        /// </summary>
        public static readonly DependencyProperty RestoreSelectedPivotProperty =
            DependencyProperty.Register(
                nameof(RestoreSelectedPivot),
                typeof(bool),
                typeof(PlayableCollectionGroupPivot),
                new PropertyMetadata(false));

        /// <summary>
        /// If true, remember and restore the last pivot that the user had selected when the control is loaded.
        /// </summary>
        public bool RestoreSelectedPivot
        {
            get => (bool)GetValue(RestoreSelectedPivotProperty);
            private set => SetValue(RestoreSelectedPivotProperty, value);
        }

        /// <summary>
        /// The backing <see cref="DependencyProperty"/> for the <see cref="PivotTitle"/> property.
        /// </summary>
        public static readonly DependencyProperty PivotTitleProperty =
            DependencyProperty.Register(
                nameof(PivotTitle),
                typeof(string),
                typeof(PlayableCollectionGroupPivot),
                new PropertyMetadata(string.Empty));

        /// <summary>
        /// If true, remember and restore the last pivot that the user had selected when the control is loaded.
        /// </summary>
        public string PivotTitle
        {
            get => (string)GetValue(PivotTitleProperty);
            set => SetValue(PivotTitleProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayableCollectionGroupPivot"/> class.
        /// </summary>
        public PlayableCollectionGroupPivot()
        {
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            AttachEvents();
        }

        private void AttachEvents()
        {
        }

        private void DetachEvents()
        {
        }

        /// <summary>
        /// The ViewModel for this control.
        /// </summary>
        public ObservableCollectionGroup ViewModel => (ObservableCollectionGroup)DataContext;
    }
}
