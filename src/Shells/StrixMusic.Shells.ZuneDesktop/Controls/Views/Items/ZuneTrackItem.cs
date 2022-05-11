using System;
using System.Collections.Generic;
using System.Text;
using OwlCore.Extensions;
using StrixMusic.Sdk.ViewModels;
using StrixMusic.Sdk.WinUI.Controls.Items;
using Windows.UI.Xaml;

namespace StrixMusic.Shells.ZuneDesktop.Controls.Views.Items
{
    /// <summary>
    /// Zune implemenation for <see cref="ZuneTrackItem"/>.
    /// </summary>
    public class ZuneTrackItem : TrackItem
    {
        /// <summary>
        /// Holds the list of artists.
        /// </summary>
        public string ArtistString
        {
            get { return (string)GetValue(ArtistStringProperty); }
            set { SetValue(ArtistStringProperty, value); }
        }

        /// <summary>
        /// Dependency property for <see cref="ArtistString" />.
        /// </summary>
        public static readonly DependencyProperty ArtistStringProperty =
            DependencyProperty.Register(nameof(ArtistString), typeof(string), typeof(ZuneTrackItem), new PropertyMetadata(null));

        /// <summary>
        /// Holds the current state of the zune <see cref="ITrackCollectionViewModel"/>.
        /// </summary>
        public ITrackCollectionViewModel ParentCollection
        {
            get { return (ITrackCollectionViewModel)GetValue(ParentCollectionProperty); }
            set { SetValue(ParentCollectionProperty, value); }
        }

        /// <summary>
        /// Dependency property for <ses cref="ITrackCollectionViewModel" />.
        /// </summary>
        public static readonly DependencyProperty ParentCollectionProperty =
            DependencyProperty.Register(nameof(ParentCollection), typeof(ITrackCollectionViewModel), typeof(ZuneTrackItem), new PropertyMetadata(null));

        /// <summary>
        /// Creates a new instance of <see cref="ZuneTrackItem"/>.
        /// </summary>
        public ZuneTrackItem()
        {
        }

        /// <inheritdoc />
        public override void OnTrackChanged(TrackViewModel? oldValue, TrackViewModel newValue)
        {
            base.OnTrackChanged(oldValue, newValue);

            if (ParentCollection is ArtistViewModel && Track?.Artists != null)
            {
                foreach (var artist in Track.Artists)
                {
                    if (artist.Name != Track.Name)
                        ArtistString += $"{artist.Name},";
                }

                if (ArtistString != null)
                    ArtistString = ArtistString.TrimEnd(',');
            }
            else
            {
                ArtistString = string.Empty;
            }
        }

        /// <inheritdoc />
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
        }
    }
}
