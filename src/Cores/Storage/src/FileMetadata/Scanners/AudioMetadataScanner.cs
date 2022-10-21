using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Extensions;
using OwlCore.Storage;
using StrixMusic.Cores.Storage.ComponentModel;
using StrixMusic.Cores.Storage.FileMetadata.Models;
using TagLib;
using File = TagLib.File;
using Logger = OwlCore.Diagnostics.Logger;

namespace StrixMusic.Cores.Storage.FileMetadata.Scanners;

/// <summary>
/// Handles extracting audio metadata from files. Includes image processing, cross-linking artists/albums/etc, and more.
/// </summary>
internal static class AudioMetadataScanner
{
    /// <summary>
    /// Scans the given files for music metadata.
    /// </summary>
    /// <param name="file">The files that will be scanned for metadata. Invalid or unsupported files will be skipped.</param>
    /// <param name="scanType">The types of scanners to use.</param>
    /// <param name="cancellationToken">A <see cref="CancellationToken"/> that will cancel the scanning task.</param>
    /// <returns>The discovered metadata from each file that is scanned.</returns>
    public static async Task<Models.FileMetadata?> ScanMusicFileAsync(IFile file, MetadataScanTypes scanType, CancellationToken cancellationToken)
    {
        var foundMetadata = new List<Models.FileMetadata>();

        if (scanType.HasFlag(MetadataScanTypes.TagLib))
        {
            var id3Metadata = await GetId3Metadata(file, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (id3Metadata is not null)
                foundMetadata.Add(id3Metadata);
        }

        if (scanType.HasFlag(MetadataScanTypes.FileProperties))
        {
            var propertyMetadata = await GetMusicFilesProperties(file, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            if (propertyMetadata is not null)
                foundMetadata.Add(propertyMetadata);
        }

        var validMetadata = foundMetadata.ToArray();
        if (validMetadata.Length == 0)
            return null;

        var aggregatedData = MergeMetadataFields(validMetadata);

        // Assign missing titles and IDs
        AssignMissingRequiredData(file, aggregatedData);

        CrossLinkMetadataIds(aggregatedData);
        cancellationToken.ThrowIfCancellationRequested();

        return aggregatedData;
    }

    internal static void AssignMissingRequiredData(IFile file, Models.FileMetadata metadata)
    {
        // If titles are missing, we leave it empty so the UI can localize the "Untitled" name.
        metadata.Id = file.Id;

        Guard.IsNotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));
        Guard.IsNotNull(metadata.TrackMetadata, nameof(metadata.TrackMetadata));
        Guard.IsNotNull(metadata.AlbumMetadata, nameof(metadata.AlbumMetadata));
        Guard.IsNotNull(metadata.AlbumArtistMetadata, nameof(metadata.AlbumArtistMetadata));
        Guard.IsNotNull(metadata.TrackArtistMetadata, nameof(metadata.TrackArtistMetadata));

        // Track
        if (string.IsNullOrWhiteSpace(metadata.TrackMetadata.Title))
            metadata.TrackMetadata.Title = string.Empty;

        metadata.TrackMetadata.Id ??= file.Id;

        metadata.TrackMetadata.ArtistIds ??= new HashSet<string>();
        metadata.TrackMetadata.ImageIds ??= new HashSet<string>();
        metadata.TrackMetadata.Genres ??= new HashSet<string>();

        // Album
        if (string.IsNullOrWhiteSpace(metadata.AlbumMetadata.Title))
            metadata.AlbumMetadata.Title = string.Empty;

        var albumId = (metadata.AlbumMetadata.Title + "_album").HashMD5Fast();
        metadata.AlbumMetadata.Id = albumId;

        metadata.AlbumMetadata.ArtistIds ??= new HashSet<string>();
        metadata.AlbumMetadata.ImageIds ??= new HashSet<string>();
        metadata.AlbumMetadata.TrackIds ??= new HashSet<string>();
        metadata.AlbumMetadata.Genres ??= new HashSet<string>();

        // Artist
        foreach (var artistMetadata in metadata.AlbumArtistMetadata)
            AssignMissingArtistData(artistMetadata);

        foreach (var artistMetadata in metadata.TrackArtistMetadata)
            AssignMissingArtistData(artistMetadata);

        void AssignMissingArtistData(ArtistMetadata artistMetadata)
        {
            if (string.IsNullOrWhiteSpace(artistMetadata.Name))
                artistMetadata.Name = string.Empty;

            var artistId = (artistMetadata.Name + "_artist").HashMD5Fast();
            artistMetadata.Id = artistId;

            artistMetadata.AlbumIds ??= new HashSet<string>();
            artistMetadata.TrackIds ??= new HashSet<string>();
            artistMetadata.ImageIds ??= new HashSet<string>();
            artistMetadata.Genres ??= new HashSet<string>();

            Guard.IsNotNullOrWhiteSpace(metadata.TrackMetadata.Id, nameof(metadata.TrackMetadata.Id));
            Guard.IsNotNullOrWhiteSpace(metadata.AlbumMetadata.Id, nameof(metadata.AlbumMetadata.Id));
            Guard.IsNotNullOrWhiteSpace(artistMetadata.Id, nameof(artistMetadata.Id));
        }
    }

    internal static Models.FileMetadata MergeMetadataFields(Models.FileMetadata[] metadata)
    {
        Guard.HasSizeGreaterThan(metadata, 0, nameof(metadata));
        if (metadata.Length == 1)
            return metadata[0];

        var primaryData = metadata[0];

        for (var i = 1; i < metadata.Length; i++)
        {
            var item = metadata[i];

            if (primaryData.TrackMetadata != null && item.TrackMetadata != null)
            {
                primaryData.TrackMetadata.TrackNumber ??= item.TrackMetadata.TrackNumber;
                primaryData.TrackMetadata.Genres ??= item.TrackMetadata.Genres;
                primaryData.TrackMetadata.DiscNumber ??= item.TrackMetadata.DiscNumber;
                primaryData.TrackMetadata.Duration ??= item.TrackMetadata.Duration;
                primaryData.TrackMetadata.Lyrics ??= item.TrackMetadata.Lyrics;
                primaryData.TrackMetadata.Language ??= item.TrackMetadata.Language;
                primaryData.TrackMetadata.Description ??= item.TrackMetadata.Description;
                primaryData.TrackMetadata.Title ??= item.TrackMetadata.Title;
                primaryData.TrackMetadata.Id ??= item.TrackMetadata.Id;
                primaryData.TrackMetadata.Year ??= item.TrackMetadata.Year;

                foreach (var imageItem in item.ImageMetadata ?? Enumerable.Empty<ImageMetadata>())
                {
                    if (imageItem is null)
                        continue;

                    Guard.IsNotNull(primaryData.TrackMetadata.ImageIds);
                    Guard.IsNotNull(imageItem.Id);
                    primaryData.TrackMetadata.ImageIds.Add(imageItem.Id);
                }
            }

            if (primaryData.AlbumMetadata != null && item.AlbumMetadata != null)
            {
                primaryData.AlbumMetadata.DatePublished ??= item.AlbumMetadata.DatePublished;
                primaryData.AlbumMetadata.Genres ??= item.AlbumMetadata.Genres;
                primaryData.AlbumMetadata.Duration ??= item.AlbumMetadata.Duration;
                primaryData.AlbumMetadata.Description ??= item.AlbumMetadata.Description;
                primaryData.AlbumMetadata.Title ??= item.AlbumMetadata.Title;

                foreach (var imageItem in item.ImageMetadata ?? Enumerable.Empty<ImageMetadata>())
                {
                    if (imageItem is null)
                        continue;

                    Guard.IsNotNull(primaryData.AlbumMetadata.ImageIds);
                    Guard.IsNotNull(imageItem.Id);
                    primaryData.AlbumMetadata.ImageIds.Add(imageItem.Id);
                }
            }

            if (primaryData.AlbumArtistMetadata != null && item.AlbumArtistMetadata != null)
            {
                foreach (var artistMetadata in primaryData.AlbumArtistMetadata)
                {
                    foreach (var artItem in item.AlbumArtistMetadata)
                    {
                        artistMetadata.Name ??= artItem.Name;
                        artistMetadata.Url ??= artItem.Url;
                    }
                }
            }
        }

        return primaryData;
    }

    /// <summary>
    /// Populates the ArtistIds, TrackIds, ImageIds, etc., with known album, track, playlist, image, etc., metadata extracted from the file.
    /// </summary>
    /// <param name="fileMetadata"></param>
    private static void CrossLinkMetadataIds(Models.FileMetadata fileMetadata)
    {
        // Each FileMetadata is the data for a single file.
        // Album and Artist IDs are generated based on Title/Name. The IDs generated here will match IDs generated for Albums/Artists from other files, if the Name matches.
        // The metadata repositories merge the linked IDs together if you add an item that already exists,
        // and HashSet will deduplicate primitive type values.
        Guard.IsNotNullOrWhiteSpace(fileMetadata.AlbumMetadata?.Id, nameof(fileMetadata.AlbumMetadata.Id));
        Guard.IsNotNullOrWhiteSpace(fileMetadata.TrackMetadata?.Id, nameof(fileMetadata.TrackMetadata.Id));

        Logger.LogInformation($"Cross-linking IDs for metadata ID {fileMetadata.Id}");

        // Albums
        Guard.IsNotNull(fileMetadata.AlbumMetadata?.ArtistIds);
        Guard.IsNotNull(fileMetadata.AlbumMetadata?.TrackIds);
        Guard.IsNotNull(fileMetadata.AlbumMetadata?.ImageIds);
        Guard.IsNotNull(fileMetadata.AlbumArtistMetadata, nameof(fileMetadata.AlbumArtistMetadata));
        Guard.IsNotNull(fileMetadata.TrackArtistMetadata, nameof(fileMetadata.TrackArtistMetadata));
        Guard.IsNotEqualTo(fileMetadata.AlbumArtistMetadata.Count, 0);
        Guard.IsNotEqualTo(fileMetadata.TrackArtistMetadata.Count, 0);

        // Albums
        fileMetadata.AlbumMetadata.TrackIds.Add(fileMetadata.TrackMetadata.Id);
            
        foreach (var artistMetadata in fileMetadata.AlbumArtistMetadata)
        {
            Guard.IsNotNull(artistMetadata.Id, nameof(artistMetadata.Id));
            fileMetadata.AlbumMetadata.ArtistIds.Add(artistMetadata.Id);
            fileMetadata.AlbumMetadata.ImageIds = new HashSet<string>(fileMetadata.ImageMetadata?.Select(x => x.Id).PruneNull() ?? Array.Empty<string>());
        }

        // Artists
        foreach (var artistMetadata in fileMetadata.AlbumArtistMetadata)
            LinkArtistMetadataIds(fileMetadata, artistMetadata);

        foreach (var artistMetadata in fileMetadata.TrackArtistMetadata)
            LinkArtistMetadataIds(fileMetadata, artistMetadata);

        // Tracks
        fileMetadata.TrackMetadata.AlbumId = fileMetadata.AlbumMetadata.Id;

        foreach (var artistMetadata in fileMetadata.TrackArtistMetadata)
            LinkTrackMetadataIds(fileMetadata, artistMetadata);

        foreach (var artistMetadata in fileMetadata.AlbumArtistMetadata)
            LinkTrackMetadataIds(fileMetadata, artistMetadata);

        static void LinkArtistMetadataIds(Models.FileMetadata metadata, ArtistMetadata artistMetadata)
        {
            Guard.IsNotNull(artistMetadata.TrackIds);
            Guard.IsNotNull(artistMetadata.AlbumIds);
            Guard.IsNotNull(metadata.TrackMetadata?.Id);
            Guard.IsNotNull(metadata.AlbumMetadata?.Id);

            artistMetadata.TrackIds.Add(metadata.TrackMetadata.Id);
            artistMetadata.AlbumIds.Add(metadata.AlbumMetadata.Id);
        }

        static void LinkTrackMetadataIds(Models.FileMetadata metadata, ArtistMetadata artistMetadata)
        {
            Guard.IsNotNull(artistMetadata.Id, nameof(artistMetadata.Id));
            Guard.IsNotNull(metadata.TrackMetadata?.ArtistIds);

            metadata.TrackMetadata.ArtistIds.Add(artistMetadata.Id);
            metadata.TrackMetadata.ImageIds = new HashSet<string>(metadata.ImageMetadata?.Select(x => x.Id).PruneNull() ?? Array.Empty<string>());
        }
    }

    /// <summary>
    /// Given an image identifier, extract the <see cref="IFolder.Id"/>.
    /// </summary>
    /// <returns>The extracted file ID.</returns>
    /// <exception cref="ArgumentException">Couldn't extract scanned image type from image ID.</exception>
    public static string GetFileIdFromImageId(string imageId)
    {
        if (imageId.EndsWith(".FileThumbnail"))
            return imageId.Replace(".FileThumbnail", string.Empty);

        if (imageId.Contains(".Id3.Image."))
        {
            // Remove all text including and after the start of the image type identifier.
            return imageId.Split(new[] { ".Id3.Image." }, 2, StringSplitOptions.None)[0];
        }

        throw new ArgumentException($"Couldn't extract scanned image type from image ID {imageId}.");
    }

    /// <summary>
    /// Gets the stream of the provided image 
    /// </summary>
    /// <param name="file">The file to extract a stream from.</param>
    /// <param name="imageId">The ID of the image to return.</param>
    /// <returns>A Task containing the image stream, if found.</returns>
    /// <exception cref="ArgumentException">Couldn't extract scanned image type from image ID.</exception>
    public static async Task<Stream?> GetImageStream(IFile file, string imageId)
    {
        // Open image thumbnail stream from AbstractStorage.
        if (imageId.EndsWith("FileThumbnail"))
        {
            if (file is not IMusicProperties musicProps)
                return null;

            // Flow.Catch doesn't accommodate async. Awaiting the task will cause exceptions to propagate.
            Stream? imageStream = null;

            try
            {
                imageStream = await musicProps.OpenMusicThumbnailStream();
            }
            catch
            {
                // Ignored
            }

            return imageStream;
        }

        // Open image stream from TagLib (at correct index)
        if (imageId.Contains("Id3.Image."))
        {
            try
            {
                using var fileStream = await file.OpenStreamAsync();

                using var tagFile = File.Create(new FileAbstraction(file.Name, fileStream), ReadStyle.Average);
                var tag = tagFile.Tag;

                // If there's no metadata to read, return null
                if (tag == null)
                {
                    Logger.LogInformation($"File {file.Id}: no metadata found.");
                    return null;
                }

                var index = int.Parse(imageId.Split(new[] { "Id3.Image." }, 2, StringSplitOptions.None)[1]);
                var targetPicture = tag.Pictures[index];

                if (targetPicture is ILazy lazy)
                    lazy.Load();

                return new MemoryStream(targetPicture.Data.Data);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message, ex);
                return null;
            }
        }

        throw new ArgumentException($"Couldn't extract scanned image type from image ID {imageId}.");
    }

    private static async Task<Models.FileMetadata?> GetMusicFilesProperties(IFile file, CancellationToken cancellationToken)
    {
        Logger.LogInformation($"{nameof(GetMusicFilesProperties)} entered for file {file.Id}");

        if (file is not IMusicProperties musicProps)
            return null;

        var storageProps = await musicProps.GetMusicPropertiesAsync(cancellationToken);
        var details = storageProps.Value;

        cancellationToken.ThrowIfCancellationRequested();

        // Flow.Catch doesn't accommodate async. Awaiting the task will cause exceptions to propagate.
        Stream? imageStream = null;

        try
        {
            imageStream = await musicProps.OpenMusicThumbnailStream();
        }
        catch
        {
            // Ignored
        }

        cancellationToken.ThrowIfCancellationRequested();

        var relatedMetadata = new Models.FileMetadata(file.Id)
        {
            AlbumMetadata = new AlbumMetadata
            {
                Title = details.Album,
                Duration = details.Duration,
                Genres = new HashSet<string>(details.Genres?.PruneNull() ?? Enumerable.Empty<string>()),
            },
            TrackMetadata = new TrackMetadata
            {
                TrackNumber = details.TrackNumber,
                Title = details.Title,
                Genres = new HashSet<string>(details.Genres?.PruneNull() ?? Enumerable.Empty<string>()),
                Duration = details.Duration,
                Year = details.Year,
            },
            AlbumArtistMetadata = new List<ArtistMetadata>
            {
                new()
                {
                    Name = details.AlbumArtist,
                    Genres = new HashSet<string>(details.Genres?.PruneNull() ?? Enumerable.Empty<string>()),
                }
            },
            TrackArtistMetadata = new List<ArtistMetadata>(),
            ImageMetadata = new List<ImageMetadata>(),
        };

        if (details.Composers is not null)
            relatedMetadata.TrackArtistMetadata.AddRange(details.Composers.Select(x => new ArtistMetadata { Name = x }));

        if (details.Conductors is not null)
            relatedMetadata.TrackArtistMetadata.AddRange(details.Conductors.Select(x => new ArtistMetadata { Name = x }));

        if (details.Producers is not null)
            relatedMetadata.TrackArtistMetadata.AddRange(details.Producers.Select(x => new ArtistMetadata { Name = x }));

        if (details.Writers is not null)
            relatedMetadata.TrackArtistMetadata.AddRange(details.Writers.Select(x => new ArtistMetadata { Name = x }));
        if (imageStream is not null)
        {
            Path.GetExtension(file.Name).TryGetMimeType(out var mimeType);

            relatedMetadata.ImageMetadata.Add(new ImageMetadata
            {
                Id = $"{file.Id}.FileThumbnail",
                MimeType = mimeType,
            });

            imageStream.Dispose();
        }

        // If no artist data, create "unknown" placeholder.
        if (relatedMetadata.AlbumArtistMetadata.Count == 0)
            relatedMetadata.AlbumArtistMetadata.Add(new ArtistMetadata { Name = string.Empty });

        // Make sure album artists are also on the track.
        foreach (var item in relatedMetadata.AlbumArtistMetadata)
        {
            if (relatedMetadata.TrackArtistMetadata.All(x => x.Name != item.Name))
                relatedMetadata.TrackArtistMetadata.Add(item);
        }

        Guard.IsNotEqualTo(relatedMetadata.AlbumArtistMetadata.Count, 0);
        Guard.IsNotEqualTo(relatedMetadata.TrackArtistMetadata.Count, 0);

        cancellationToken.ThrowIfCancellationRequested();

        return relatedMetadata;
    }

    public static async Task<Models.FileMetadata?> GetId3Metadata(IFile file, CancellationToken cancellationToken)
    {
        Logger.LogInformation($"{nameof(GetId3Metadata)} entered for file {file.Id}");

        try
        {
            using var stream = await file.OpenStreamAsync(FileAccess.ReadWrite, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            if (stream.CanSeek)
                stream.Seek(0, SeekOrigin.Begin);

            TagLibHelper.TryAddManualFileTypeResolver();

            Logger.LogInformation($"Loading {file.Name} with TagLib");

            try
            {
                using var tagFile = File.Create(new FileAbstraction(file.Name, stream), ReadStyle.Average);
                var tag = tagFile.Tag;

                cancellationToken.ThrowIfCancellationRequested();

                // If there's no metadata to read, return null
                if (tag == null || tag.IsEmpty)
                {
                    Logger.LogInformation($"File {file.Id}: no metadata found.");
                    return null;
                }

                var imageMetadata = new List<ImageMetadata>();

                for (var index = 0; index < tag.Pictures.Length; index++)
                {
                    var picture = tag.Pictures[index];

                    /* Height and width can be read and supplied here, but it requires reading the picture into memory.
                       This can be very slow, and has been disabled.
                       Re-enable when the user is able to opt-in to the feature.
                        
                     using var imageFile = File.Create(new FileAbstraction($"image{picture.MimeType.GetExtensionFromMime()}", new MemoryStream(picture.Data.Data)), ReadStyle.Average) as TagLib.Image.File;
                    if (imageFile is null)
                        continue;

                    var height = imageFile.Properties.PhotoHeight;
                    var width = imageFile.Properties.PhotoWidth;
                    */

                    imageMetadata.Add(new ImageMetadata
                    {
                        Id = $"{file.Id}.Id3.Image.{index}",
                        MimeType = picture.MimeType
                    });
                }

                var fileMetadata = new Models.FileMetadata(file.Id)
                {
                    AlbumMetadata = new AlbumMetadata
                    {
                        Description = tag.Description,
                        Title = tag.Album,
                        Duration = tagFile.Properties?.Duration,
                        Genres = new HashSet<string>(tag.Genres),
                        DatePublished = tag.DateTagged,
                        ArtistIds = new HashSet<string>(),
                        TrackIds = new HashSet<string>(),
                        ImageIds = new HashSet<string>(),
                    },
                    TrackMetadata = new TrackMetadata
                    {
                        Id = file.Id,
                        Description = tag.Description,
                        Title = tag.Title,
                        DiscNumber = tag.Disc,
                        Duration = tagFile.Properties?.Duration,
                        Genres = new HashSet<string>(tag.Genres),
                        TrackNumber = tag.Track,
                        Year = tag.Year,
                        ArtistIds = new HashSet<string>(),
                        ImageIds = new HashSet<string>(),
                    },
                    AlbumArtistMetadata = new List<ArtistMetadata>(tag.AlbumArtists.Select(x => new ArtistMetadata
                    {
                        Name = x,
                        Genres = new HashSet<string>(tag.Genres)
                    })),
                    TrackArtistMetadata = new List<ArtistMetadata>(tag.Performers.Select(x => new ArtistMetadata
                    {
                        Name = x,
                        Genres = new HashSet<string>(tag.Genres)
                    })),
                    ImageMetadata = imageMetadata,
                };

                // If no artist data, create "unknown" placeholder.
                if (fileMetadata.AlbumArtistMetadata.Count == 0)
                    fileMetadata.AlbumArtistMetadata.Add(new ArtistMetadata { Name = string.Empty });

                // Make sure album artists are also on the track.
                foreach (var item in fileMetadata.AlbumArtistMetadata)
                {
                    if (fileMetadata.TrackArtistMetadata.All(x => x.Name != item.Name))
                        fileMetadata.TrackArtistMetadata.Add(item);
                }

                Guard.IsNotEqualTo(fileMetadata.AlbumArtistMetadata.Count, 0);
                Guard.IsNotEqualTo(fileMetadata.TrackArtistMetadata.Count, 0);

                cancellationToken.ThrowIfCancellationRequested();

                Logger.LogInformation($"File {file.Id}: Metadata scan completed.");
                return fileMetadata;
            }
            catch (Exception ex)
            {
                Logger.LogError($"{ex}");
            }
        }
        catch (CorruptFileException ex)
        {
            Logger.LogError($"{nameof(CorruptFileException)} for file {file.Id}", ex);
        }
        catch (UnsupportedFormatException ex)
        {
            Logger.LogError($"{nameof(UnsupportedFormatException)} for file {file.Id}", ex);
        }
        catch (FileLoadException ex)
        {
            Logger.LogError($"{nameof(FileLoadException)} for file {file.Id}", ex);
        }
        catch (FileNotFoundException ex)
        {
            Logger.LogError($"{nameof(FileNotFoundException)} for file {file.Id}", ex);
        }
        catch (ArgumentException ex)
        {
            Logger.LogError($"{nameof(ArgumentException)} for file {file.Id}", ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            Logger.LogError($"{nameof(UnauthorizedAccessException)} for file {file.Id}", ex);
        }

        return null;
    }
}
