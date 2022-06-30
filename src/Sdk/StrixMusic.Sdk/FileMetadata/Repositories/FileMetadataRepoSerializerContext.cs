// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System.Collections.Generic;
using System.Text.Json.Serialization;
using StrixMusic.Sdk.FileMetadata.Models;

namespace StrixMusic.Sdk.FileMetadata.Repositories
{
    /// <summary>
    /// Supplies type information for file metadata models.
    /// </summary>
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(Models.FileMetadata))]
    [JsonSerializable(typeof(AlbumMetadata))]
    [JsonSerializable(typeof(ArtistMetadata))]
    [JsonSerializable(typeof(PlaylistMetadata))]
    [JsonSerializable(typeof(TrackMetadata))]
    [JsonSerializable(typeof(ImageMetadata))]
    [JsonSerializable(typeof(Lyrics))]
    [JsonSerializable(typeof(List<Models.FileMetadata>))]
    [JsonSerializable(typeof(List<AlbumMetadata>))]
    [JsonSerializable(typeof(List<ArtistMetadata>))]
    [JsonSerializable(typeof(List<PlaylistMetadata>))]
    [JsonSerializable(typeof(List<TrackMetadata>))]
    [JsonSerializable(typeof(List<ImageMetadata>))]
    internal partial class FileMetadataRepoSerializerContext : JsonSerializerContext
    {
    }
}
