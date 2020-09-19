using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using StrixMusic.Sdk.Enums;
using StrixMusic.Sdk.Events;

namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Metadata about a track.
    /// </summary>
    public interface ITrack : IArtistCollection
    {
        /// <inheritdoc cref="TrackType"/>
        TrackType Type { get; }

        /// <summary>
        /// An <see cref="IAlbum"/> object that this track belongs to.
        /// </summary>
        IAlbum? Album { get; }

        /// <summary>
        /// A list of <see cref="string"/> describing the genres for this track.
        /// </summary>
        IReadOnlyList<string>? Genres { get; }

        /// <summary>
        /// Position in a set, usually the album.
        /// </summary>
        int? TrackNumber { get; }

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
        IPlayableCollectionGroup? RelatedItems { get; }

        /// <summary>
        /// If true, <see cref="ChangeArtistsAsync(IReadOnlyList{IArtist}?)"/> is supported.
        /// </summary>
        bool IsChangeArtistsAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeAlbumAsync(IAlbum?)"/> is supported.
        /// </summary>
        bool IsChangeAlbumAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeGenresAsync(IReadOnlyList{string}?)"/> is supported.
        /// </summary>
        bool IsChangeGenresAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeTrackNumberAsync(int?)"/> is supported.
        /// </summary>
        bool IsChangeTrackNumberAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeLanguageAsync(CultureInfo)"/> is supported.
        /// </summary>
        bool IsChangeLanguageAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeLyricsAsync(ILyrics?)"/> is supported.
        /// </summary>
        bool IsChangeLyricsAsyncSupported { get; }

        /// <summary>
        /// If true, <see cref="ChangeIsExplicitAsync(bool)"/> is supported.
        /// </summary>
        bool IsChangeIsExplicitAsyncSupported { get; }

        /// <summary>
        /// Changes the <see cref="IArtistCollection.Artists"/> for this track.
        /// </summary>
        /// <param name="artists">Artist</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeArtistsAsync(IReadOnlyList<IArtist>? artists);

        /// <summary>
        /// Changes the <see cref="Album"/> for this track.
        /// </summary>
        /// <param name="albums">The new album.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeAlbumAsync(IAlbum? albums);

        /// <summary>
        /// Change the <see cref="Genres"/> for this track.
        /// </summary>
        /// <param name="genres">The new genres.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeGenresAsync(IReadOnlyList<string>? genres);

        /// <summary>
        /// Changes the <see cref="TrackNumber"/> on this track.
        /// </summary>
        /// <param name="trackNumber">The new track number.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeTrackNumberAsync(int? trackNumber);

        /// <summary>
        /// Changes the <see cref="Language"/> for this track.
        /// </summary>
        /// <param name="language">The new language for this track.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeLanguageAsync(CultureInfo language);

        /// <summary>
        /// Changes the <see cref="Lyrics"/> for this track.
        /// </summary>
        /// <param name="lyrics">The new lyrics data.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeLyricsAsync(ILyrics? lyrics);

        /// <summary>
        /// Changes the <see cref="IsExplicit"/> for this track.
        /// </summary>
        /// <param name="isExplicit">The new value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeIsExplicitAsync(bool isExplicit);

        /// <summary>
        /// Fires when the <see cref="Genres"/> metadata changes.
        /// </summary>
        event EventHandler<CollectionChangedEventArgs<string>> GenresChanged;

        /// <summary>
        /// Fires when the <see cref="Album"/> metadata changes.
        /// </summary>
        event EventHandler<IAlbum?> AlbumChanged;

        /// <summary>
        /// Fires when the <see cref="TrackNumber"/> metadata changes.
        /// </summary>
        event EventHandler<int?> TrackNumberChanged;

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
