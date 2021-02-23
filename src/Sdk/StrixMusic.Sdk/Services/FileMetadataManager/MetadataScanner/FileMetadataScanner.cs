using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
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
        private readonly INotificationService _notificationService;
        private TaskCompletionSource<List<FileMetadata>>? _folderScanningTaskCompletion;
        private int _filesFound;
        private int _filesProcessed;
        private AbstractProgressUIElement? _progressUIElement;

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

            _notificationService = Ioc.Default.GetRequiredService<INotificationService>();

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
        /// Raised when a previously scanned file has been updated with new information.
        /// </summary>
        public event EventHandler<FileMetadata>? FileMetadataUpdated;

        /// <summary>
        /// Raised when a previously scanned file has been removed from the file system.
        /// </summary>
        public event EventHandler<FileMetadata>? FileMetadataRemoved;

        private int FilesFound
        {
            get => _filesFound;
            set
            {
                _filesFound = value;

                if (_progressUIElement != null)
                    _progressUIElement.Maximum = value;
            }
        }

        private int FilesProcessed
        {
            get => _filesProcessed;
            set
            {
                _filesProcessed = value;

                if (_progressUIElement != null)
                    _progressUIElement.Value = value;
            }
        }

        private static IPicture? GetAlbumArt(IPicture[] pics)
        {
            var albumPictureTypesRanking = new[]
            {
                PictureType.FrontCover,
                PictureType.Illustration,
                PictureType.Other,
                PictureType.Media,
                PictureType.MovieScreenCapture,
                PictureType.DuringPerformance,
                PictureType.DuringRecording,
                PictureType.Artist,
                PictureType.LeadArtist,
                PictureType.Band,
                PictureType.BandLogo,
                PictureType.Composer,
                PictureType.Conductor,
                PictureType.RecordingLocation,
                PictureType.FileIcon,
                PictureType.OtherFileIcon,
            };

            foreach (var type in albumPictureTypesRanking)
            {
                foreach (var sourcePic in pics)
                {
                    if (sourcePic.Type == type)
                        return sourcePic;
                }
            }

            return null;
        }

        private static IPicture? GetArtistArt(IPicture[] pics)
        {
            var artistPictureTypesRanking = new[]
            {
                PictureType.Artist,
                PictureType.LeadArtist,
                PictureType.FrontCover,
                PictureType.Illustration,
                PictureType.Other,
                PictureType.Media,
                PictureType.MovieScreenCapture,
                PictureType.DuringPerformance,
                PictureType.DuringRecording,
                PictureType.DuringPerformance,
                PictureType.DuringRecording,
                PictureType.Band,
                PictureType.BandLogo,
                PictureType.Composer,
                PictureType.Conductor,
                PictureType.RecordingLocation,
                PictureType.FileIcon,
                PictureType.OtherFileIcon,
            };

            foreach (var type in artistPictureTypesRanking)
            {
                foreach (var sourcePic in pics)
                {
                    if (sourcePic.Type == type)
                        return sourcePic;
                }
            }

            return null;
        }

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
            mergeTrackMetadata.Id ??= fileData.Path.HashMD5Fast();

            // If titles are missing, we leave it empty so the UI can localize the "Untitled" name
            if (mergeTrackMetadata.AlbumMetadata != null && string.IsNullOrWhiteSpace(mergeTrackMetadata.AlbumMetadata.Title))
                mergeTrackMetadata.AlbumMetadata.Title = string.Empty;

            if (mergeTrackMetadata.TrackMetadata != null && string.IsNullOrWhiteSpace(mergeTrackMetadata.TrackMetadata.Title))
                mergeTrackMetadata.TrackMetadata.Title = string.Empty;

            if (mergeTrackMetadata.ArtistMetadata != null && string.IsNullOrWhiteSpace(mergeTrackMetadata.ArtistMetadata.Name))
                mergeTrackMetadata.ArtistMetadata.Name = string.Empty;

            if (mergeTrackMetadata.TrackMetadata != null && mergeTrackMetadata.TrackMetadata.ImagePath is null)
            {
                // TODO get file thumbnail
            }

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
                Uri? artistImagePath = null;

                if (tags.Pictures != null)
                {
                    var albumArt = GetAlbumArt(tags.Pictures);

                    if (albumArt != null)
                    {
                        byte[] imageData = albumArt.Data.Data;

                        var imageFile = await CacheFolder.CreateFileAsync($"{fileData.DisplayName}.thumb", CreationCollisionOption.ReplaceExisting);
                        await imageFile.WriteAllBytesAsync(imageData);

                        imagePath = new Uri(imageFile.Path);
                    }

                    var artistPic = GetArtistArt(tags.Pictures);

                    if (artistPic != null)
                    {
                        byte[] imageData = artistPic.Data.Data;

                        var imageFile = await CacheFolder.CreateFileAsync($"{fileData.DisplayName}_artist.thumb", CreationCollisionOption.ReplaceExisting);
                        await imageFile.WriteAllBytesAsync(imageData);

                        artistImagePath = new Uri(imageFile.Path);
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
                        ImagePath = artistImagePath,
                        Genres = new List<string>(tags.Genres),
                    },
                };
            }
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
        /// <param name="fileData">The file that was scanned to get the <paramref name="relatedMetadata"/>.</param>
        private FileMetadata? ApplyRelatedMetadataIds(FileMetadata relatedMetadata, IFileData fileData)
        {
            // TODO clean up this method and the two related artist and album methods.
            lock (relatedMetadata)
            {
                var trackMetadata = relatedMetadata.TrackMetadata;
                var albumMetadata = relatedMetadata.AlbumMetadata;
                var artistMetadata = relatedMetadata.ArtistMetadata;

                if (trackMetadata is null || albumMetadata is null || artistMetadata is null)
                    return null;

                trackMetadata.Id = (fileData.Path + ".track").HashMD5Fast();

                Guard.IsNotNullOrWhiteSpace(trackMetadata.Id, nameof(trackMetadata.Id));

                if (_fileMetadata.Count == 0)
                {
                    var albumId = (fileData.Path + ".album").HashMD5Fast();

                    albumMetadata.Id = albumId;

                    var artistId = (fileData.Path + ".artist").HashMD5Fast();
                    artistMetadata.Id = artistId;
                    trackMetadata.AlbumId = albumId;

                    albumMetadata.TrackIds ??= new List<string>();
                    albumMetadata.TrackIds.Add(trackMetadata.Id);

                    return relatedMetadata;
                }

                artistMetadata = ApplyRelatedArtistMetadata(artistMetadata, trackMetadata, fileData);
                albumMetadata = ApplyRelatedAlbumMetadata(albumMetadata, trackMetadata, artistMetadata, fileData);

                relatedMetadata.AlbumMetadata = albumMetadata;
                relatedMetadata.ArtistMetadata = artistMetadata;
                relatedMetadata.TrackMetadata = trackMetadata;

                trackMetadata.AlbumId = albumMetadata.Id;
                trackMetadata.ArtistIds ??= new List<string>();

                if (artistMetadata?.Id != null)
                    trackMetadata.ArtistIds.Add(artistMetadata.Id);

                return relatedMetadata;
            }
        }

        private ArtistMetadata ApplyRelatedArtistMetadata(ArtistMetadata artistMetadata, TrackMetadata trackMetadata, IFileData fileData)
        {
            Guard.IsNotNull(trackMetadata.Id, nameof(trackMetadata.Id));

            var existingArtist = _fileMetadata.FirstOrDefault(c => c.ArtistMetadata?.Name?.Equals(artistMetadata.Name, StringComparison.OrdinalIgnoreCase) ?? false)?.ArtistMetadata;

            if (existingArtist != null)
            {
                existingArtist.TrackIds ??= new List<string>();
                existingArtist.TrackIds.Add(trackMetadata.Id);

                artistMetadata = existingArtist;

                _ = Task.Run(() => FileMetadataUpdated?.Invoke(this, new FileMetadata() { ArtistMetadata = existingArtist, TrackMetadata = trackMetadata }));
            }
            else
            {
                var artistId = (fileData.Path + ".artist").HashMD5Fast();
                artistMetadata.Id = artistId;

                artistMetadata.TrackIds = new List<string>
                {
                    trackMetadata.Id,
                };
            }

            return artistMetadata;
        }

        private AlbumMetadata ApplyRelatedAlbumMetadata(AlbumMetadata albumMetadata, TrackMetadata trackMetadata, ArtistMetadata artistMetadata, IFileData fileData)
        {
            Guard.IsNotNull(trackMetadata.Id, nameof(trackMetadata.Id));

            var existingAlbum = _fileMetadata.FirstOrDefault(c =>
                    c.AlbumMetadata?.Title?.Equals(albumMetadata.Title, StringComparison.OrdinalIgnoreCase) ?? false)
                ?.AlbumMetadata;

            if (existingAlbum != null)
            {
                existingAlbum.TrackIds ??= new List<string>();
                existingAlbum.TrackIds.Add(trackMetadata.Id);

                albumMetadata = existingAlbum;
                albumMetadata.ArtistIds ??= new List<string>();

                if (artistMetadata?.Id != null)
                {
                    if (!albumMetadata.ArtistIds?.Contains(artistMetadata.Id) ?? false)
                    {
                        albumMetadata.ArtistIds?.Add(artistMetadata.Id);
                    }
                }

                FileMetadataUpdated?.Invoke(this, new FileMetadata { AlbumMetadata = existingAlbum, TrackMetadata = trackMetadata, ArtistMetadata = artistMetadata });
            }
            else
            {
                var albumId = (fileData.Path + ".album").HashMD5Fast();
                albumMetadata.Id = albumId;

                albumMetadata.TrackIds = new List<string>();
                albumMetadata.ArtistIds = new List<string>();

                albumMetadata.TrackIds.Add(trackMetadata.Id);

                if (artistMetadata?.Id != null)
                    albumMetadata.ArtistIds.Add(artistMetadata.Id);
            }

            return albumMetadata;
        }

        /// <summary>
        /// Gets all unique albums. Make sure file metadata is already scanned.
        /// </summary>
        /// <returns>A list of unique <see cref="AlbumMetadata"/></returns>
        public async Task<IReadOnlyList<AlbumMetadata>> GetUniqueAlbumMetadata()
        {
            if (_folderScanningTaskCompletion != null && _folderScanningTaskCompletion.Task.Status != TaskStatus.RanToCompletion)
                await _folderScanningTaskCompletion.Task;

            lock (_fileMetadata)
            {
                var albums = _fileMetadata.Select(c => c.AlbumMetadata).PruneNull();

                return albums.DistinctBy(c => c?.Id).ToList();
            }
        }

        /// <summary>
        /// Gets all unique artist.
        /// </summary>
        /// <returns>A list of unique <see cref="ArtistMetadata"/></returns>
        public async Task<IReadOnlyList<ArtistMetadata>> GetUniqueArtistMetadata()
        {
            if (_folderScanningTaskCompletion != null && _folderScanningTaskCompletion.Task.Status != TaskStatus.RanToCompletion)
                await _folderScanningTaskCompletion.Task;

            lock (_fileMetadata)
            {
                var artists = _fileMetadata.Select(c => c.ArtistMetadata).PruneNull();

                return artists.DistinctBy(c => c?.Id).ToList();
            }
        }

        /// <summary>
        /// Gets all unique tracks. Make sure file meta is already scanned.
        /// </summary>
        /// <returns>A list of unique <see cref="ArtistMetadata"/></returns>
        public async Task<IReadOnlyList<TrackMetadata>> GetUniqueTrackMetadata()
        {
            if (_folderScanningTaskCompletion != null && _folderScanningTaskCompletion.Task.Status != TaskStatus.RanToCompletion)
                await _folderScanningTaskCompletion.Task;

            lock (_fileMetadata)
            {
                var tracks = _fileMetadata.Select(c => c.TrackMetadata).PruneNull();

                return tracks.DistinctBy(c => c?.Id).ToList();
            }
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

            IsInitialized = true;

            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));

            await ScanFolderForMetadata();
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
            var dfsNotification = RaiseStructureNotification();

            var allDiscoveredFolders = new Queue<IFolderData>();
            var allDiscoveredFiles = new Queue<IFileData>();
            var foldersToScan = new Stack<IFolderData>();
            foldersToScan.Push(_folderData);
            allDiscoveredFolders.Enqueue(_folderData);

            await DFSFolderContentScan(foldersToScan, allDiscoveredFolders, allDiscoveredFiles);

            dfsNotification.Dismiss();

            var contentScanNotification = RaiseProcessingNotification();

            Parallel.ForEach(allDiscoveredFiles, file =>
           {
               _ = ProcessFile(file).Result;
           });

            contentScanNotification.Dismiss();

            lock (_fileMetadata)
            {
                var result = _fileMetadata.ToList();

                _folderScanningTaskCompletion?.SetResult(result);
            }
        }

        private async Task DFSFolderContentScan(Stack<IFolderData> foldersToCrawl, Queue<IFolderData> allDiscoveredFolders, Queue<IFileData> filesToScan)
        {
            if (foldersToCrawl.Count == 0)
                return;

            IFolderData folderData = foldersToCrawl.Pop();

            var files = await folderData.GetFilesAsync();
            var filesList = files.ToList();

            foreach (var file in filesList)
            {
                filesToScan.Enqueue(file);
            }

            FilesFound += filesList.Count;

            var folders = await folderData.GetFoldersAsync();

            foreach (var folder in folders)
            {
                foldersToCrawl.Push(folder);
                allDiscoveredFolders.Enqueue(folder);
            }

            await DFSFolderContentScan(foldersToCrawl, allDiscoveredFolders, filesToScan);
        }

        private async Task<FileMetadata?> ProcessFile(IFileData file)
        {
            if (!_supportedMusicFileFormats.Contains(file.FileExtension))
            {
                FilesFound--;
                return null;
            }

            var metadata = await ScanFileMetadata(file);

            if (metadata == null)
                return null;

            lock (_fileMetadata)
            {
                metadata = ApplyRelatedMetadataIds(metadata, file);

                if (metadata != null)
                    _fileMetadata.Add(metadata);

                FilesProcessed++;

                if (metadata != null)
                {
                    _ = Task.Run(() => FileMetadataAdded?.Invoke(this, metadata));
                }
            }

            return metadata;
        }

        private Notification RaiseStructureNotification()
        {
            static string NewGuid() => Guid.NewGuid().ToString();

            var elementGroup = new AbstractUIElementGroup(NewGuid())
            {
                Title = "Scanning folder structure",
                Subtitle = $"Scanning folder tree at {_folderData.Path}",
                Items = { new AbstractProgressUIElement(NewGuid(), null) },
            };

            return _notificationService.RaiseNotification(elementGroup);
        }

        private Notification RaiseProcessingNotification()
        {
            static string NewGuid() => Guid.NewGuid().ToString();

            _progressUIElement = new AbstractProgressUIElement(NewGuid(), FilesProcessed, FilesFound);

            var elementGroup = new AbstractUIElementGroup(NewGuid())
            {
                Title = "Scanning folder contents",
                Subtitle = $"Processing {FilesFound} files in {_folderData.Path}",
                Items = { _progressUIElement },
            };

            return _notificationService.RaiseNotification(elementGroup);
        }
    }
}
