﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using StrixMusic.Sdk.Services.Notifications;
using TagLib;
using File = TagLib.File;

namespace StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner
{
    /// <summary>
    /// Handles scanning tracks for all supported metadata, including notification support for when data changes.
    /// </summary>
    public class FileMetadataScanner : IAsyncInit, IDisposable
    {
        private readonly string[] _supportedMusicFileFormats = { ".mp3", ".flac", ".m4a" };
        private readonly List<FileMetadata> _fileMetadata = new List<FileMetadata>();
        private readonly IFolderData _folderData;
        private TaskCompletionSource<List<FileMetadata>>? _folderScanningTaskCompletion;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The folder to use for storing file metadata.
        /// </summary>
        public IFolderData? CacheFolder { get; internal set; }

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataScanner"/>.
        /// </summary>
        /// <param name="rootFolder">The root folder to operate in when scanning. Will be scanned recursively.</param>
        public FileMetadataScanner(IFolderData rootFolder)
        {
            _folderData = rootFolder;

            AttachEvents();
        }

        private void AttachEvents()
        {
            // todo subscribe to file system changes.
        }

        private void DetachEvents()
        {
        }

        /// <summary>
        /// Raised when a new file with metadata is discovered.
        /// </summary>
        public event EventHandler<FileMetadata>? FileMetadataAdded;

        /// <summary>
        /// Raised when a previously scanned file has been removed from the file system.
        /// </summary>
        public event EventHandler<FileMetadata>? FileMetadataRemoved;

        private async Task<FileMetadata?> ScanFileMetadata(IFileData fileData)
        {
            var id3Metadata = await GetID3Metadata(fileData);

            // disabled for now, scanning non-songs returns valid data
            // var propertyMetadata = await GetMusicFilesProperties(fileData);
            var foundMetadata = new[] { id3Metadata };

            var validMetadata = foundMetadata.PruneNull().ToArray();

            if (validMetadata.Length == 0)
                return null;

            var mergeTrackMetadata = MergeTrackMetadata(validMetadata);
            mergeTrackMetadata.Id = Guid.NewGuid().ToString();

            return mergeTrackMetadata;
        }

        private FileMetadata MergeTrackMetadata(FileMetadata[] metadata)
        {
            if (metadata.Length == 1)
                return metadata[0];

            var mergedMetaData = metadata[0];
            for (var i = 1; i < metadata.Length; i++)
            {
                var item = metadata[i];

                if (mergedMetaData.TrackMetadata != null && item.TrackMetadata != null)
                {
                    mergedMetaData.TrackMetadata.TrackNumber ??= item.TrackMetadata.TrackNumber;
                    mergedMetaData.TrackMetadata.Genres ??= item.TrackMetadata.Genres;
                    mergedMetaData.TrackMetadata.DiscNumber ??= item.TrackMetadata.DiscNumber;
                    mergedMetaData.TrackMetadata.Duration ??= item.TrackMetadata.Duration;
                    mergedMetaData.TrackMetadata.Lyrics ??= item.TrackMetadata.Lyrics;
                    mergedMetaData.TrackMetadata.Language ??= item.TrackMetadata.Language;
                    mergedMetaData.TrackMetadata.Description ??= item.TrackMetadata.Description;
                    mergedMetaData.TrackMetadata.Title ??= item.TrackMetadata.Title;
                    mergedMetaData.TrackMetadata.Url ??= item.TrackMetadata.Url;
                    mergedMetaData.TrackMetadata.Year ??= item.TrackMetadata.Year;
                }

                if (mergedMetaData.AlbumMetadata != null && item.AlbumMetadata != null)
                {
                    mergedMetaData.AlbumMetadata.DatePublished ??= item.AlbumMetadata.DatePublished;
                    mergedMetaData.AlbumMetadata.Genres ??= item.AlbumMetadata.Genres;
                    mergedMetaData.AlbumMetadata.Duration ??= item.AlbumMetadata.Duration;
                    mergedMetaData.AlbumMetadata.Description ??= item.AlbumMetadata.Description;
                    mergedMetaData.AlbumMetadata.Title ??= item.AlbumMetadata.Title;
                }

                if (mergedMetaData.ArtistMetadata != null && item.ArtistMetadata != null)
                {
                    mergedMetaData.ArtistMetadata.Name ??= item.ArtistMetadata.Name;
                    mergedMetaData.ArtistMetadata.Url ??= item.ArtistMetadata.Url;
                }
            }

            return mergedMetaData;
        }

        private async Task<FileMetadata?> GetMusicFilesProperties(IFileData fileData)
        {
            var details = await fileData.Properties.GetMusicPropertiesAsync();

            if (details is null)
                return null;

            var relatedMetadata = new FileMetadata();
            relatedMetadata.AlbumMetadata = new AlbumMetadata()
            {
                Title = details.Album,
                Duration = details.Duration,
                Genres = new List<string>(details.Genre),
            };

            relatedMetadata.TrackMetadata = new TrackMetadata()
            {
                TrackNumber = details.TrackNumber,
                Title = details.Title,
                Genres = details.Genre?.ToList(),
                Duration = details.Duration,
                Source = new Uri(fileData.Path),
                Year = details.Year,
            };

            relatedMetadata.ArtistMetadata = new ArtistMetadata()
            {
                Name = details.Artist,
            };

            return relatedMetadata;
        }

        private async Task<FileMetadata?> GetID3Metadata(IFileData fileData)
        {
            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));

            try
            {
                using var stream = await fileData.GetStreamAsync();

                using var tagFile = File.Create(new FileAbstraction(fileData.Name, stream), ReadStyle.Average);

                // Read the raw tags
                var tags = tagFile.GetTag(TagTypes.Id3v2);

                // If there's no metadata to read, return null
                if (tags == null)
                    return null;

                Uri? imagePath = null;

                if (tags.Pictures != null && tags.Pictures.Length > 0)
                {
                    var albumArt = tags.Pictures.FirstOrDefault(p => p.Type == PictureType.FrontCover);

                    if (albumArt != null)
                    {
                        byte[] imageData = albumArt.Data.Data;

                        var imageFile = await CacheFolder.CreateFileAsync(fileData.Name);
                        await imageFile.WriteAllBytesAsync(imageData);
                        
                        imagePath = new Uri(imageFile.Path);
                    }
                }

                return new FileMetadata
                {
                    AlbumMetadata = new AlbumMetadata
                    {
                        Description = tags.Description,
                        Title = tags.Album,
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        DatePublished = tags.DateTagged,
                        TotalTracksCount = Convert.ToInt32(tags.TrackCount),
                        TotalArtistsCount = tags.AlbumArtists.Length,
                    },
                    TrackMetadata = new TrackMetadata
                    {
                        Source = new Uri(fileData.Path),
                        ImagePath = imagePath,
                        Description = tags.Description,
                        Title = tags.Title,
                        DiscNumber = tags.Disc,
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        TrackNumber = tags.Track,
                        Year = tags.Year,
                    },
                    ArtistMetadata = new ArtistMetadata
                    {
                        Name = tags.FirstAlbumArtist,
                    },
                };
            }

            // Catches have a big performance hit
            catch (CorruptFileException)
            {
                return null;
            }
            catch (UnsupportedFormatException)
            {
                return null;
            }
        }

        /// <summary>
        /// Create groups and apply ids to all data in a <see cref="List{RelatedMetadata}"/>.
        /// </summary>
        /// <param name="relatedMetadata">The data to parse.</param>
        private void ApplyRelatedMetadataIds(List<FileMetadata> relatedMetadata)
        {
            var artistGroup = relatedMetadata.GroupBy(c => c.ArtistMetadata?.Name);
            var albumGroup = relatedMetadata.GroupBy(c => c.AlbumMetadata?.Title);
            var trackGroup = relatedMetadata.GroupBy(c => c.TrackMetadata?.Title);

            foreach (var tracks in albumGroup)
            {
                var albumId = Guid.NewGuid().ToString();
                foreach (var item in tracks)
                {
                    if (item.AlbumMetadata != null && item.TrackMetadata != null && item.ArtistMetadata != null)
                    {
                        item.AlbumMetadata.Id = albumId;
                        item.TrackMetadata.AlbumId = albumId;
                        item.ArtistMetadata.AlbumIds = new List<string> { albumId };
                    }
                }
            }

            foreach (var tracks in artistGroup)
            {
                var artistId = Guid.NewGuid().ToString();
                foreach (var item in tracks)
                {
                    if (item.AlbumMetadata != null && item.TrackMetadata != null && item.ArtistMetadata != null)
                    {
                        item.ArtistMetadata.Id = artistId;
                        item.TrackMetadata.ArtistIds = new List<string> { artistId };
                        item.AlbumMetadata.ArtistIds = new List<string> { artistId };
                    }
                }
            }

            foreach (var tracks in trackGroup)
            {
                var trackId = Guid.NewGuid().ToString();
                foreach (var item in tracks)
                {
                    if (item.AlbumMetadata != null && item.TrackMetadata != null && item.ArtistMetadata != null)
                    {
                        item.TrackMetadata.Id = trackId;
                        item.AlbumMetadata.TrackIds = new List<string> { trackId };
                        item.ArtistMetadata.TrackIds = new List<string> { trackId };
                    }
                }
            }
        }

        /// <summary>
        /// Gets all unique albums. Make sure file metadata is already scanned.
        /// </summary>
        /// <returns>A list of unique <see cref="AlbumMetadata"/></returns>
        public async Task<IReadOnlyList<AlbumMetadata>> GetUniqueAlbumMetadata()
        {
            if (_folderScanningTaskCompletion != null)
                await _folderScanningTaskCompletion.Task;

            var albums = _fileMetadata.Select(c => c.AlbumMetadata).PruneNull();

            return albums.DistinctBy(c => c?.Id).ToList();
        }

        /// <summary>
        /// Gets all unique artist.
        /// </summary>
        /// <returns>A list of unique <see cref="ArtistMetadata"/></returns>
        public async Task<IReadOnlyList<ArtistMetadata>> GetUniqueArtistMetadata()
        {
            if (_folderScanningTaskCompletion != null)
                await _folderScanningTaskCompletion.Task;

            // todo check and throw if metadata isn't scanned
            var artists = _fileMetadata.Select(c => c.ArtistMetadata).PruneNull();

            return artists.DistinctBy(c => c?.Id).ToList();
        }

        /// <summary>
        /// Gets all unique tracks. Make sure file meta is already scanned.
        /// </summary>
        /// <returns>A list of unique <see cref="ArtistMetadata"/></returns>
        public async Task<IReadOnlyList<TrackMetadata>> GetUniqueTrackMetadata()
        {
            if (_folderScanningTaskCompletion != null)
                await _folderScanningTaskCompletion.Task;

            var tracks = _fileMetadata.Select(c => c.TrackMetadata).PruneNull();

            return tracks.DistinctBy(c => c?.Id).ToList();
        }

        private void ReleaseUnmanagedResources()
        {
            DetachEvents();
        }

        /// <inheritdoc cref="Dispose()"/>
        protected virtual void Dispose(bool disposing)
        {
            if (!IsInitialized)
                return;

            ReleaseUnmanagedResources();
            if (disposing)
            {
                // dispose any objects you created here
            }

            IsInitialized = false;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        ~FileMetadataScanner()
        {
            Dispose(false);
        }

        /// <inheritdoc />
        public async Task InitAsync()
        {
            if (IsInitialized)
                return;

            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));

            await ScanFolderForMetadata();

            IsInitialized = true;
        }

        /// <summary>
        /// Scans a folder and all subfolders for music and music metadata.
        /// </summary>
        /// <returns>Fully scanned <see cref="IReadOnlyList{RelatedMetadata}"/>.</returns>
        private async Task ScanFolderForMetadata()
        {
            _folderScanningTaskCompletion = new TaskCompletionSource<List<FileMetadata>>();

            // DFS search on a single thread
            // Parallel.ForEach on resulting collection (system manages resources)
            // Batch the scanned metadata at the end (in the event)
            var notificationService = Ioc.Default.GetRequiredService<INotificationService>();

            // notifications disabled until notification templates are reasonable and don't get in the way
            var dfsNotification = notificationService.RaiseNotification("Scanning folder structure", $"Scanning folder tree at {_folderData.Path}");

            var allDiscoveredFolders = new Queue<IFolderData>();
            var foldersToScan = new Stack<IFolderData>();
            foldersToScan.Push(_folderData);
            allDiscoveredFolders.Enqueue(_folderData);

            await DFSFolderContentScan(foldersToScan, allDiscoveredFolders);
            
            dfsNotification.Dismiss();

            var contentScanNotification = notificationService.RaiseNotification("Scanning folder contents", $"Processing data in {_folderData.Path}");

            await allDiscoveredFolders.InParallel(ProcessFolderContents);

            contentScanNotification.Dismiss();

            var result = _fileMetadata.ToList();

            _folderScanningTaskCompletion?.SetResult(result);
            _folderScanningTaskCompletion = null;
        }

        private async Task DFSFolderContentScan(Stack<IFolderData> foldersToCrawl, Queue<IFolderData> allDiscoveredFolders)
        {
            if (foldersToCrawl.Count == 0)
                return;

            var folders = await foldersToCrawl.Pop().GetFoldersAsync();

            foreach (var folder in folders)
            {
                foldersToCrawl.Push(folder);
                allDiscoveredFolders.Enqueue(folder);
            }

            await DFSFolderContentScan(foldersToCrawl, allDiscoveredFolders);
        }

        private async Task ProcessFolderContents(IFolderData folder)
        {
            var files = await folder.GetFilesAsync();

            foreach (var file in files)
            {
                var metadata = await ProcessFile(file);

                if (metadata != null)
                {
                    lock (_fileMetadata)
                    {
                        _fileMetadata.Add(metadata);
                        ApplyRelatedMetadataIds(_fileMetadata);
                    }

                    // TODO: Major issue. If any related metadata is updated but already emitted, we need to update the data externally as well.
                    FileMetadataAdded?.Invoke(this, metadata);
                }
            }
        }

        private Task<FileMetadata?> ProcessFile(IFileData file)
        {
            if (!_supportedMusicFileFormats.Contains(file.FileExtension))
                return Task.FromResult<FileMetadata?>(null);

            return ScanFileMetadata(file);
        }
    }
}