using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using StrixMusic.Cores.Storage.FileMetadata.Models;

namespace StrixMusic.Cores.Storage.FileMetadata.Repositories;

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
[JsonSerializable(typeof(ConcurrentDictionary<string, Models.FileMetadata>))]
[JsonSerializable(typeof(List<AlbumMetadata>))]
[JsonSerializable(typeof(List<ArtistMetadata>))]
[JsonSerializable(typeof(List<PlaylistMetadata>))]
[JsonSerializable(typeof(List<TrackMetadata>))]
[JsonSerializable(typeof(List<ImageMetadata>))]
internal partial class FileMetadataRepoSerializerContext : JsonSerializerContext
{
}
