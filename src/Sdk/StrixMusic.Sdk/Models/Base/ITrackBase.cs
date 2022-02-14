// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Globalization;
using System.Threading.Tasks;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Represents an audio stream with metadata that belongs to an <see cref="ITrackCollectionBase"/>.
    /// </summary>
    public interface ITrackBase : IPlayableCollectionItem, IArtistCollectionBase, IGenreCollectionBase, IAsyncDisposable
    {
        /// <inheritdoc cref="TrackType"/>
        TrackType Type { get; }

        /// <summary>
        /// Position in the album.
        /// </summary>
        /// <remarks>
        /// If an album has several discs, the track number is the number on the specified disc.
        /// </remarks>
        int? TrackNumber { get; }

        /// <summary>
        /// The disc number (usually 1 unless the album consists of more than one disc).
        /// </summary>
        int? DiscNumber { get; }

        /// <summary>
        /// The language for this track.
        /// </summary>
        /// <remarks>If track has no spoken words (instrumental), value is <see cref="CultureInfo.InvariantCulture"/>. If unknown, value is <see langword="null"/>.</remarks>
        CultureInfo? Language { get; }

        /// <summary>
        /// If this track contains explicit language.
        /// </summary>
        bool IsExplicit { get; }

        /// <summary>
        /// If true, changing albums is supported.
        /// </summary>
        bool IsChangeAlbumAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeTrackNumberAsync(int?)"/> is supported.
        /// </summary>
        bool IsChangeTrackNumberAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeLanguageAsync(CultureInfo)"/> is supported.
        /// </summary>
        bool IsChangeLanguageAsyncAvailable { get; }

        /// <summary>
        /// If true, changing lyrics is supported.
        /// </summary>
        bool IsChangeLyricsAsyncAvailable { get; }

        /// <summary>
        /// If true, <see cref="ChangeIsExplicitAsync(bool)"/> is supported.
        /// </summary>
        bool IsChangeIsExplicitAsyncAvailable { get; }

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
        /// Changes the <see cref="IsExplicit"/> for this track.
        /// </summary>
        /// <param name="isExplicit">The new value.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        Task ChangeIsExplicitAsync(bool isExplicit);

        /// <summary>
        /// Fires when the <see cref="TrackNumber"/> metadata changes.
        /// </summary>
        event EventHandler<int?>? TrackNumberChanged;

        /// <summary>
        /// Fires when the <see cref="Language"/> metadata changes.
        /// </summary>
        event EventHandler<CultureInfo?>? LanguageChanged;

        /// <summary>
        /// Fires when the <see cref="IsExplicit"/> metadata changes.
        /// </summary>
        event EventHandler<bool>? IsExplicitChanged;
    }
}