// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using StrixMusic.Sdk.Plugins.Model;

namespace StrixMusic.Sdk.Plugins.PopulateEmptyNames;

/// <summary>
/// A plugin that intercepts null or whitespaces names on albums, artist, tracks, playlists and other Playable items, and uses a provided value instead (when provided).
/// </summary>
public class PopulateEmptyNamesPlugin : SdkModelPlugin
{
    private readonly static ModelPluginMetadata _metadata = new(
        id: nameof(PopulateEmptyNamesPlugin),
        displayName: "Populate empty names",
        description: "Provides a fallback value for items that are missing a name.",
        sdkVer: typeof(SdkModelPlugins).Assembly.GetName().Version);

    private string _emptyAlbumName = string.Empty;
    private string _emptyArtistName = string.Empty;
    private string _emptyPlaylistName = string.Empty;
    private string _emptyTrackName = string.Empty;
    private string _emptyDefaultName = "?";

    /// <summary>
    /// Initializes a new instance of the <see cref="PopulateEmptyNamesPlugin"/> class.
    /// </summary>
    public PopulateEmptyNamesPlugin()
        : base(_metadata)
    {
    }

    /// <summary>
    /// The name to use instead for an album when empty.
    /// </summary>
    public string EmptyAlbumName
    {
        get => _emptyAlbumName;
        set
        {
            Album.Clear();
            if (!string.IsNullOrWhiteSpace(value))
                Album.Add(x => new PopulateEmptyAlbumNamePlugin(_metadata, x, value));

            _emptyAlbumName = value;
        }
    }

    /// <summary>
    /// The name to use instead for an artist when empty.
    /// </summary>
    public string EmptyArtistName
    {
        get => _emptyArtistName;
        set
        {
            Artist.Clear();
            if (!string.IsNullOrWhiteSpace(value))
                Artist.Add(x => new PopulateEmptyArtistNamePlugin(_metadata, x, value));

            _emptyArtistName = value;
        }
    }

    /// <summary>
    /// The name to use instead for a playlist when empty.
    /// </summary>
    public string EmptyPlaylistName
    {
        get => _emptyPlaylistName;
        set
        {
            Playlist.Clear();
            if (!string.IsNullOrWhiteSpace(value))
                Playlist.Add(x => new PopulateEmptyPlaylistNamePlugin(_metadata, x, value));

            _emptyPlaylistName = value;
        }
    }

    /// <summary>
    /// The name to use instead for a track when empty.
    /// </summary>
    public string EmptyTrackName
    {
        get => _emptyTrackName;
        set
        {
            Track.Clear();
            if (!string.IsNullOrWhiteSpace(value))
                Track.Add(x => new PopulateEmptyTrackNamePlugin(_metadata, x, value));

            _emptyTrackName = value;
        }
    }

    /// <summary>
    /// The name to use instead for all other playable items when empty.
    /// </summary>
    public string EmptyDefaultName
    {
        get => _emptyDefaultName;
        set
        {
            Playable.Clear();
            if (!string.IsNullOrWhiteSpace(value))
                Playable.Add(x => new PopulateEmptyPlayableNamePlugin(_metadata, x, value));

            _emptyDefaultName = value;
        }
    }
}
