using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using StrixMusic.Sdk.Services.Notifications;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TagLib;
using File = TagLib.File;

namespace StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner
{
    /// <summary>
    /// Handles scanning tracks for all supported metadata, including notification support for when data changes.
    /// </summary>
    public class FileMetadataScanner : IAsyncInit, IDisposable
    {
        private const int BUCKET_SIZE = 20;
        private static readonly string[] _supportedMusicFileFormats = { ".mp3", ".flac", ".m4a", ".wma" };
        private static readonly string[] _supportedPlaylistFileFormats = { ".zpl", ".wpl", ".smil", ".m3u", ".m3u8", ".vlc", ".xspf", ".asx", ".mpcpl", ".fpl", ".pls", ".aimppl4" };
        private static readonly PictureType[] _albumPictureTypesRanking =
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

        private static readonly PictureType[] _artistPictureTypesRanking =
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

        private readonly string _emitDebouncerId = Guid.NewGuid().ToString();
        private readonly List<FileMetadata> _batchMetadataToEmit = new List<FileMetadata>();
        private readonly List<FileMetadata> _fileMetadatas = new List<FileMetadata>();
        private readonly SemaphoreSlim _batchLock;
        private readonly IFolderData _folderData;
        private readonly CancellationTokenSource _scanningCancellationTokenSource;

        private int _mediaFilesFound;
        private int _filesProcessed;

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataScanner"/>.
        /// </summary>
        /// <param name="rootFolder">The root folder to operate in when scanning. Will be scanned recursively.</param>
        public FileMetadataScanner(IFolderData rootFolder)
        {
            _folderData = rootFolder;

            _batchLock = new SemaphoreSlim(1, 1);
            _scanningCancellationTokenSource = new CancellationTokenSource();

            AttachEvents();
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

        private void AttachEvents()
        {
            // todo subscribe to file system changes.
        }

        private void DetachEvents()
        {
            // todo unsubscribe to file system changes.
        }

        /// <summary>
        /// Raised when a new file with metadata is discovered.
        /// </summary>
        public event EventHandler<IEnumerable<FileMetadata>>? FileMetadataAdded;

        /// <summary>
        /// Raised when a previously scanned file has been removed from the file system.
        /// </summary>
        public event EventHandler<IEnumerable<FileMetadata>>? FileMetadataRemoved;

        /// <summary>
        /// Raised when a new file is found.
        /// </summary>
        public event EventHandler? FilesFoundCountUpdated;

        /// <summary>
        /// Raised when a new file is processed.
        /// </summary>
        public event EventHandler? FilesProcessedCountUpdated;

        /// <summary>
        /// Raised file discovery starts
        /// </summary>
        public event EventHandler? FileDiscoveryStarted;

        /// <summary>
        /// Raised when all files are found.
        /// </summary>
        public event EventHandler<IEnumerable<IFileData>>? FileDiscoveryCompleted;

        /// <summary>
        /// Raised when all file scanning is complete.
        /// </summary>
        public event EventHandler<IEnumerable<FileMetadata>>? FileScanCompleted;


        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// The folder to use for storing file metadata.
        /// </summary>
        public IFolderData? CacheFolder { get; internal set; }

        private static IPicture? GetAlbumArt(IPicture[] pics)
        {
            foreach (var type in _albumPictureTypesRanking)
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
            foreach (var type in _artistPictureTypesRanking)
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

            if (metadata.ArtistMetadata == null)
                return;

            if (string.IsNullOrWhiteSpace(metadata.ArtistMetadata.Name))
                metadata.ArtistMetadata.Name = string.Empty;

            var artistId = (metadata.ArtistMetadata.Name ?? string.Empty + ".artist").HashMD5Fast();
            metadata.ArtistMetadata.Id = artistId;
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
            var id3Metadata = await GetId3Metadata(fileData);

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

        private async Task<FileMetadata?> GetId3Metadata(IFileData fileData)
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
        /// Scans a folder and all subfolders for music and music metadata.
        /// </summary>
        /// <returns>Fully scanned <see cref="IReadOnlyList{RelatedMetadata}"/>.</returns>
        private async Task ScanFolderForMetadata()
        {
            var allDiscoveredFiles = new Queue<IFileData>();
            var foldersToScan = new Stack<IFolderData>();
            foldersToScan.Push(_folderData);

            try
            {
                FileDiscoveryStarted?.Invoke(this, EventArgs.Empty);

                await Task.Run(() => DFSFolderContentScan(foldersToScan, allDiscoveredFiles));

                FileDiscoveryCompleted?.Invoke(this, allDiscoveredFiles);
            }
            catch (OperationCanceledException)
            {
                _scanningCancellationTokenSource.Dispose();
                return;
            }

            var musicFiles = allDiscoveredFiles.Where(c => _supportedMusicFileFormats.Contains(c.FileExtension));

            try
            {
                if (_scanningCancellationTokenSource.Token.IsCancellationRequested)
                    _scanningCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var fileDatas = musicFiles.ToList();

                var bucketCount = Math.Ceiling((double)fileDatas.Count / BUCKET_SIZE);

                var offset = 0;
                for (var i = 0; i < bucketCount; i++)
                {
                    if (_scanningCancellationTokenSource.Token.IsCancellationRequested)
                        _scanningCancellationTokenSource.Token.ThrowIfCancellationRequested();

                    var files = fileDatas.GetRange(offset, BUCKET_SIZE);

                    await files.InParallel(ProcessFile);

                    offset += BUCKET_SIZE;
                }
            }
            catch (OperationCanceledException)
            {
                _scanningCancellationTokenSource.Dispose();
            }
        }

        private async Task DFSFolderContentScan(Stack<IFolderData> foldersToCrawl, Queue<IFileData> filesToScan)
        {
            while (foldersToCrawl.Count > 0)
            {
                if (_scanningCancellationTokenSource.Token.IsCancellationRequested)
                    _scanningCancellationTokenSource.Token.ThrowIfCancellationRequested();

                var folderData = foldersToCrawl.Pop();

                var files = await folderData.GetFilesAsync();
                var filesList = files.ToList();

                foreach (var file in filesList)
                {
                    if (_supportedMusicFileFormats.Contains(file.FileExtension) || _supportedPlaylistFileFormats.Contains(file.FileExtension))
                    {
                        if (_supportedMusicFileFormats.Contains(file.FileExtension))
                        {
                            _mediaFilesFound++;
                        }

                        if (_scanningCancellationTokenSource.Token.IsCancellationRequested)
                            _scanningCancellationTokenSource.Token.ThrowIfCancellationRequested();

                        //considering playlists files too. Because we are all scannable files one time using FileMetadataScanner.
                        FilesFoundCountUpdated?.Invoke(this, EventArgs.Empty);
                        filesToScan.Enqueue(file);
                    }
                }

                var folders = await folderData.GetFoldersAsync();

                foreach (var folder in folders)
                {
                    foldersToCrawl.Push(folder);
                }
            }
        }

        private async Task<FileMetadata?> ProcessFile(IFileData file)
        {
            var fileMetadata = new FileMetadata();

            if (_supportedMusicFileFormats.Contains(file.FileExtension))
                fileMetadata = await ProcessMusicFile(file);

            if (_scanningCancellationTokenSource.Token.IsCancellationRequested)
                _scanningCancellationTokenSource.Token.ThrowIfCancellationRequested();

            if (_scanningCancellationTokenSource.Token.IsCancellationRequested)
                _scanningCancellationTokenSource.Token.ThrowIfCancellationRequested();

            _filesProcessed++;

            FilesProcessedCountUpdated?.Invoke(this, EventArgs.Empty);

            await _batchLock.WaitAsync();

            if (fileMetadata != null)
                _batchMetadataToEmit.Add(fileMetadata);

            _batchLock.Release();

            _ = HandleChanged();

            return fileMetadata;
        }

        private async Task<FileMetadata?> ProcessMusicFile(IFileData file)
        {
            var metadata = await ScanFileMetadata(file);

            return metadata ?? null;
        }

        private async Task HandleChanged()
        {
            if (_filesProcessed != _mediaFilesFound && _batchMetadataToEmit.Count < 100 && !await Flow.Debounce(_emitDebouncerId, TimeSpan.FromSeconds(5)))
                return;

            await _batchLock.WaitAsync();

            if (_scanningCancellationTokenSource.Token.IsCancellationRequested)
                _scanningCancellationTokenSource.Token.ThrowIfCancellationRequested();

            FileMetadataAdded?.Invoke(this, _batchMetadataToEmit.ToArray());

            _fileMetadatas.AddRange(_batchMetadataToEmit);

            _batchMetadataToEmit.Clear();

            _batchLock.Release();

            if (_filesProcessed == _mediaFilesFound)
                FileScanCompleted?.Invoke(this, _fileMetadatas);
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

            if (disposing)
            {
                // dispose any objects you created here
                ReleaseUnmanagedResources();
                _scanningCancellationTokenSource.Cancel();
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
    }
}
