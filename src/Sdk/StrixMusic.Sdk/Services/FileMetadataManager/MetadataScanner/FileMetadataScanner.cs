using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
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
        private readonly string[] _supportedMusicFileFormats = { ".mp3", ".flac", ".m4a", ".wma" };
        private readonly ConcurrentBag<FileMetadata> _fileMetadata = new ConcurrentBag<FileMetadata>();
        private readonly IFolderData _folderData;
        private readonly INotificationService _notificationService;
        private int _filesFound;
        private int _filesProcessed;
        private AbstractProgressUIElement? _progressUIElement;
        private Notification? _filesScannedNotification;
        private Notification? _filesFoundNotification;

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

                UpdateFilesFoundNotification();
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

                UpdateFilesScanNotification();
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

        private static void AssignMissingRequiredData(IFileData fileData, FileMetadata metadata)
        {
            metadata.Id ??= fileData.Path.HashMD5Fast();

            if (metadata.AlbumMetadata != null)
            {
                if (string.IsNullOrWhiteSpace(metadata.AlbumMetadata.Title))
                    metadata.AlbumMetadata.Title = string.Empty;

                var albumId = (metadata.AlbumMetadata.Title ?? string.Empty + ".album").HashMD5Fast();
                metadata.AlbumMetadata.Id = albumId;
            }

            if (metadata.TrackMetadata != null)
            {
                if (string.IsNullOrWhiteSpace(metadata.TrackMetadata.Title))
                    metadata.TrackMetadata.Title = string.Empty;

                metadata.TrackMetadata.Id ??= fileData.Path.HashMD5Fast();

                if (metadata.TrackMetadata.ImagePath is null)
                {
                    // TODO get file thumbnail
                }
            }

            if (metadata.ArtistMetadata != null)
            {
                if (string.IsNullOrWhiteSpace(metadata.ArtistMetadata.Name))
                    metadata.ArtistMetadata.Name = string.Empty;

                var artistId = (metadata.ArtistMetadata.Name ?? string.Empty + ".artist").HashMD5Fast();
                metadata.ArtistMetadata.Id = artistId;
            }
        }

        private static FileMetadata MergeMetadataFields(FileMetadata[] metadata)
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
                    primaryData.TrackMetadata.Url ??= item.TrackMetadata.Url;
                    primaryData.TrackMetadata.Year ??= item.TrackMetadata.Year;
                }

                if (primaryData.AlbumMetadata != null && item.AlbumMetadata != null)
                {
                    primaryData.AlbumMetadata.DatePublished ??= item.AlbumMetadata.DatePublished;
                    primaryData.AlbumMetadata.Genres ??= item.AlbumMetadata.Genres;
                    primaryData.AlbumMetadata.Duration ??= item.AlbumMetadata.Duration;
                    primaryData.AlbumMetadata.Description ??= item.AlbumMetadata.Description;
                    primaryData.AlbumMetadata.Title ??= item.AlbumMetadata.Title;
                }

                if (primaryData.ArtistMetadata != null && item.ArtistMetadata != null)
                {
                    primaryData.ArtistMetadata.Name ??= item.ArtistMetadata.Name;
                    primaryData.ArtistMetadata.Url ??= item.ArtistMetadata.Url;
                }
            }

            return primaryData;
        }

        private async Task<FileMetadata?> ScanFileMetadata(IFileData fileData)
        {
            var id3Metadata = await GetID3Metadata(fileData);

            // Disabled for now, UI is getting duplicate info (also may not use)
            //var propertyMetadata = await GetMusicFilesProperties(fileData);
            var foundMetadata = new[] { id3Metadata };
            var validMetadata = foundMetadata.PruneNull().ToArray();
            if (validMetadata.Length == 0)
                return null;

            var aggregatedData = MergeMetadataFields(validMetadata);

            // Assign missing titles and IDs
            // If titles are missing, we leave it empty so the UI can localize the "Untitled" name
            AssignMissingRequiredData(fileData, aggregatedData);

            return aggregatedData;
        }

        private async Task<FileMetadata?> GetMusicFilesProperties(IFileData fileData)
        {
            var details = await fileData.Properties.GetMusicPropertiesAsync();

            if (details is null)
                return null;

            var relatedMetadata = new FileMetadata
            {
                AlbumMetadata = new AlbumMetadata
                {
                    Title = details.Album,
                    Duration = details.Duration,
                    Genres = details.Genres?.ToOrAsList(),
                },
                TrackMetadata = new TrackMetadata
                {
                    TrackNumber = details.TrackNumber,
                    Title = details.Title,
                    Genres = details.Genres?.ToOrAsList(),
                    Duration = details.Duration,
                    Source = new Uri(fileData.Path),
                    Year = details.Year,
                },
                ArtistMetadata = new ArtistMetadata
                {
                    Name = details.Artist,
                },
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
                var tags = tagFile.GetTag(TagTypes.Id3v2) ??
                           tagFile.GetTag(TagTypes.Id3v1) ??
                           tagFile.GetTag(TagTypes.Asf) ??
                           tagFile.GetTag(TagTypes.FlacMetadata);

                // If there's no metadata to read, return null
                if (tags == null)
                    return null;

                string? imagePath = null;
                string? artistImagePath = null;

                if (tags.Pictures != null)
                {
                    var albumArt = GetAlbumArt(tags.Pictures);

                    if (albumArt != null)
                    {
                        byte[] imageData = albumArt.Data.Data;

                        try
                        {
                            var imageFile = await CacheFolder.CreateFileAsync($"{fileData.Path.HashMD5Fast()}.thumb", CreationCollisionOption.ReplaceExisting);

                            await imageFile.WriteAllBytesAsync(imageData);

                            imagePath = imageFile.Path;
                        }
                        catch (FileLoadException)
                        {
                            // Catch "The file is in use." error.
                        }
                    }

                    var artistPic = GetArtistArt(tags.Pictures);

                    if (artistPic?.Type == albumArt?.Type)
                    {
                        artistImagePath = imagePath;
                    }
                    else if (artistPic != null)
                    {
                        byte[] imageData = artistPic.Data.Data;

                        try
                        {
                            var imageFile = await CacheFolder.CreateFileAsync($"{fileData.Path.HashMD5Fast()}_artist.thumb", CreationCollisionOption.ReplaceExisting);
                            await imageFile.WriteAllBytesAsync(imageData);

                            artistImagePath = imageFile.Path;
                        }
                        catch (FileLoadException)
                        {
                            // Catch "The file is in use." error.
                        }
                    }
                }

                return new FileMetadata
                {
                    AlbumMetadata = new AlbumMetadata
                    {
                        Description = tags.Description,
                        Title = tags.Album,
                        ImagePath = imagePath,
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        DatePublished = tags.DateTagged,
                        ArtistIds = new List<string>(),
                        TrackIds = new List<string>(),
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
                        ArtistIds = new List<string>(),
                    },
                    ArtistMetadata = new ArtistMetadata
                    {
                        Name = tags.FirstAlbumArtist,
                        ImagePath = artistImagePath,
                        Genres = new List<string>(tags.Genres),
                        AlbumIds = new List<string>(),
                        TrackIds = new List<string>(),
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
            catch (FileLoadException)
            {
                return null;
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (ArgumentException)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets all albums that have been found so far.
        /// </summary>
        /// <returns>A list of unique <see cref="AlbumMetadata"/>s.</returns>
        public Task<IReadOnlyList<AlbumMetadata>> GetAlbumMetadata()
        {
            var albums = _fileMetadata.Select(c => c.AlbumMetadata).PruneNull();
            return Task.FromResult<IReadOnlyList<AlbumMetadata>>(albums.ToList());
        }

        /// <summary>
        /// Gets all artist that have been found so far.
        /// </summary>
        /// <returns>A list of unique <see cref="ArtistMetadata"/>s.</returns>
        public Task<IReadOnlyList<ArtistMetadata>> GetArtistMetadata()
        {
            var artists = _fileMetadata.Select(c => c.ArtistMetadata).PruneNull();
            return Task.FromResult<IReadOnlyList<ArtistMetadata>>(artists.ToList());
        }

        /// <summary>
        /// Gets all tracks that have been found so far.
        /// </summary>
        /// <returns>A list of unique <see cref="TrackMetadata"/>s.</returns>
        public Task<IReadOnlyList<TrackMetadata>> GetTrackMetadata()
        {
            var tracks = _fileMetadata.Select(c => c.TrackMetadata).PruneNull();
            return Task.FromResult<IReadOnlyList<TrackMetadata>>(tracks.ToList());
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

            await Task.Run(ScanFolderForMetadata);
        }

        /// <summary>
        /// Scans a folder and all subfolders for music and music metadata.
        /// </summary>
        /// <returns>Fully scanned <see cref="IReadOnlyList{RelatedMetadata}"/>.</returns>
        private async Task ScanFolderForMetadata()
        {
            // DFS search on a single thread
            // Parallel.ForEach on resulting collection (system manages resources)
            // Batch the scanned metadata at the end (in the event)
            _filesFoundNotification = RaiseStructureNotification();

            var allDiscoveredFiles = new Queue<IFileData>();
            var foldersToScan = new Stack<IFolderData>();
            foldersToScan.Push(_folderData);

            await DFSFolderContentScan(foldersToScan, allDiscoveredFiles);

            _filesFoundNotification.Dismiss();

            _filesScannedNotification = RaiseProcessingNotification();

            // Parallel scanning is disabled due to excessive GC's freezing the UI thread after moving relational code into Repositories.
            // await allDiscoveredFiles.InParallel(ProcessFile);
            while (allDiscoveredFiles.Count > 0)
            {
                var file = allDiscoveredFiles.Dequeue();
                await ProcessFile(file);
            }

            _filesScannedNotification.Dismiss();
        }

        private async Task DFSFolderContentScan(Stack<IFolderData> foldersToCrawl, Queue<IFileData> filesToScan)
        {
            while (foldersToCrawl.Count > 0)
            {
                var folderData = foldersToCrawl.Pop();

                var files = await folderData.GetFilesAsync();
                var filesList = files.ToList();

                foreach (var file in filesList)
                {
                    if (_supportedMusicFileFormats.Contains(file.FileExtension))
                    {
                        filesToScan.Enqueue(file);
                    }
                }

                FilesFound += filesList.Count;

                var folders = await folderData.GetFoldersAsync();

                foreach (var folder in folders)
                {
                    foldersToCrawl.Push(folder);
                }
            }
        }

        private async Task ProcessFile(IFileData file)
        {
            var metadata = await ScanFileMetadata(file);
            if (metadata == null)
            {
                FilesProcessed++;
                return;
            }

            lock (_fileMetadata)
            {
                _fileMetadata.Add(metadata);
            }

            FilesProcessed++;

            FileMetadataAdded?.Invoke(this, metadata);
        }

        private Notification RaiseStructureNotification()
        {
            static string NewGuid() => Guid.NewGuid().ToString();

            var elementGroup = new AbstractUIElementGroup(NewGuid())
            {
                Title = "Looking for files",
                Subtitle = $"Found {FilesFound} in {_folderData.Path}",
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
                Title = "Scanning files",
                Subtitle = $"Scanned {FilesProcessed}/{FilesFound} files in {_folderData.Path}",
                Items = { _progressUIElement },
            };

            return _notificationService.RaiseNotification(elementGroup);
        }

        private void UpdateFilesScanNotification()
        {
            Guard.IsNotNull(_filesScannedNotification, nameof(_filesScannedNotification));

            _filesScannedNotification.AbstractUIElementGroup.Subtitle = $"Processing {FilesProcessed}/{FilesFound} files in {_folderData.Path}";
        }

        private void UpdateFilesFoundNotification()
        {
            Guard.IsNotNull(_filesFoundNotification, nameof(_filesFoundNotification));

            _filesFoundNotification.AbstractUIElementGroup.Subtitle = $"Found {FilesFound} in {_folderData.Path}";
        }
    }
}
