﻿using StrixMusic.Sdk.Uno.Controls.Collections.Abstract;
using StrixMusic.Sdk.Uno.Controls.Items;
using StrixMusic.Sdk.ViewModels;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace StrixMusic.Sdk.Uno.Controls.Collections
{
    /// <summary>
    /// A Templated <see cref="Control"/> for displaying an <see cref="ITrackCollectionViewModel"/>.
    /// </summary>
    public partial class TrackCollection : CollectionControl<TrackViewModel, TrackItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrackCollection"/> class.
        /// </summary>
        public TrackCollection()
        {
            DefaultStyleKey = typeof(TrackCollection);
        }

        /// <summary>
        /// Backing dependency property for <see cref="Collection"/>.
        /// </summary>
        public ITrackCollectionViewModel Collection
        {
            get { return (ITrackCollectionViewModel)GetValue(CollectionProperty); }
            set { SetValue(CollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="ITrackCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty CollectionProperty =
            DependencyProperty.Register(nameof(Collection), typeof(ITrackCollectionViewModel), typeof(TrackCollection), new PropertyMetadata(0));

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            // OnApplyTemplate is often a more appropriate point to deal with
            // adjustments to the template-created visual tree than is the Loaded event.
            // The Loaded event might occur before the template is applied,
            // and the visual tree might be incomplete as of Loaded.
            base.OnApplyTemplate();

            AttachHandlers();
        }

        /// <inheritdoc/>
        protected override async Task LoadMore()
        {
            if (Collection == null)
                return;

            if (!Collection.PopulateMoreTracksCommand.IsRunning)
                await Collection.PopulateMoreTracksCommand.ExecuteAsync(25);
        }

        /// <inheritdoc/>
        protected override void CheckAndToggleEmpty()
        {
            if (Collection == null)
                return;

            if (!Collection.PopulateMoreTracksCommand.IsRunning &&
                Collection.TotalTrackCount == 0)
                
            SetEmptyVisibility(Visibility.Visible);
        }

        private void AttachHandlers()
        {
            Unloaded += TrackCollection_Unloaded;
        }

        private void DetachHandlers()
        {
            Unloaded -= TrackCollection_Unloaded;
        }

        private void TrackCollection_Unloaded(object sender, RoutedEventArgs e)
        {
            DetachHandlers();
        }
    }
}
