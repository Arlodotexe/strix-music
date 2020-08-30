using System;
using System.Collections.Generic;
using System.Globalization;
using StrixMusic.CoreInterfaces.Enums;

namespace StrixMusic.CoreInterfaces.Interfaces
{
    /// <summary>
    /// Metadata about a track.
    /// </summary>
    public interface ITrack : IPlayable
    {
        /// <inheritdoc cref="TrackType"/>
        TrackType Type { get; }

        /// <summary>
        /// A list of <see cref="IArtist"/>s that this track was created by.
        /// </summary>
        IReadOnlyList<IArtist> Artists { get; }

        /// <summary>
        /// An <see cref="IAlbum"/> object that this track belongs to.
        /// </summary>
        IAlbum? Album { get; }

        /// <summary>
        /// The date the track was released.
        /// </summary>
        DateTime? DatePublished { get; }

        /// <summary>
        /// A list of <see cref="string"/> describing the genres for this track.
        /// </summary>
        IReadOnlyList<string>? Genres { get; }

        /// <summary>
        /// Position in a set, usually the album.
        /// </summary>
        int? TrackNumber { get; }

        /// <summary>
        /// Number of the times this track has been played.
        /// </summary>
        int? PlayCount { get; }

        /// <summary>
        /// The language for this track.
        /// </summary>
        /// <remarks>If track has no spoken words (instrumental), value is <see cref="CultureInfo.InvariantCulture"/>. If unknown, value is <see langword="null"/>.</remarks>
        CultureInfo? Language { get; }

        /// <summary>
        /// The lyrics for this track.
        /// </summary>
        ILyrics? Lyrics { get; }

        /// <summary>
        /// If this track contains explicit language.
        /// </summary>
        bool IsExplicit { get; }

        /// <summary>
        /// A <see cref="IPlayable"/> of items related to this item.
        /// </summary>
        IPlayableCollectionGroup RelatedItems { get; }

        /// <summary>
        /// Fires when the <see cref="Artists"/> metadata changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<IArtist>> ArtistsChanged;

        /// <summary>
        /// Fires when the <see cref="Genres"/> metadata changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<string>> GenresChanged;

        /// <summary>
        /// Fires when the <see cref="Album"/> metadata changes.
        /// </summary>
        event EventHandler<IAlbum?> AlbumChanged;

        /// <summary>
        /// Fires when the <see cref="DatePublished"/> metadata changes.
        /// </summary>
        event EventHandler<DateTime?> DatePublishedChanged;

        /// <summary>
        /// Fires when the <see cref="TrackNumber"/> metadata changes.
        /// </summary>
        event EventHandler<int?> TrackNumberChanged;

        /// <summary>
        /// Fires when the <see cref="PlayCount"/> metadata changes.
        /// </summary>
        event EventHandler<int?> PlayCountChanged;

        /// <summary>
        /// Fires when the <see cref="Language"/> metadata changes.
        /// </summary>
        event EventHandler<CultureInfo?> LanguageChanged;

        /// <summary>
        /// Fires when the <see cref="Lyrics"/> metadata changes.
        /// </summary>
        event EventHandler<ILyrics?> LyricsChanged;

        /// <summary>
        /// Fires when the <see cref="IsExplicit"/> metadata changes.
        /// </summary>
        event EventHandler<bool> IsExplicitChanged;
    }
}
