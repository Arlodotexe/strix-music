using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Toolkit.Diagnostics;
using Microsoft.Toolkit.Mvvm.DependencyInjection;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using TagLib;

namespace StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner
{
    /// <summary>
    /// Handles extracting audio metadata from files. Includes image processing, cross-linking artists/albums/etc, and more.
    /// </summary>
    public sealed partial class AudioMetadataScanner : IDisposable
    {
        private static readonly string[] _supportedMusicFileFormats = { ".mp3", ".flac", ".m4a", ".wma", ".ogg" };
        private readonly int _scanBatchSize;

        private readonly FileMetadataManager _metadataManager;
        private readonly ILogger<AudioMetadataScanner> _logger;
        private readonly SemaphoreSlim _batchLock;

        private readonly string _emitDebouncerId = Guid.NewGuid().ToString();
        private readonly HashSet<FileMetadata> _batchMetadataToEmit = new HashSet<FileMetadata>();
        private readonly HashSet<FileMetadata> _allFileMetadata = new HashSet<FileMetadata>();

        private CancellationTokenSource? _scanningCancellationTokenSource;
        private int _filesToScanCount;
        private int _filesProcessed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioMetadataScanner"/> class.
        /// </summary>
        /// <param name="metadataManager">The metadata manager that handles this scanner.</param>
        public AudioMetadataScanner(FileMetadataManager metadataManager)
        {
            _logger = Ioc.Default.GetRequiredService<ILogger<AudioMetadataScanner>>();
            _metadataManager = metadataManager;
            _scanBatchSize = metadataManager.DegreesOfParallelism;

            _batchLock = new SemaphoreSlim(1, 1);
            _ongoingImageProcessingTasksSemaphore = new SemaphoreSlim(1, 1);
            _ongoingImageProcessingSemaphore = new SemaphoreSlim(_scanBatchSize, _scanBatchSize);
            _ongoingImageProcessingTasks = new ConcurrentDictionary<string, Task<IEnumerable<ImageMetadata>>>();

            AttachEvents();
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
        // ReSharper disable once UnusedMember.Global
#pragma warning disable 67 
        public event EventHandler<IEnumerable<FileMetadata>>? FileMetadataRemoved;
#pragma warning restore 67

        /// <summary>
        /// Raised when all file scanning is complete.
        /// </summary>
        public event EventHandler<IEnumerable<FileMetadata>>? FileScanCompleted;

        /// <summary>
        /// The folder to use for storing file metadata.
        /// </summary>
        public IFolderData? CacheFolder { get; internal set; }

        /// <summary>
        /// Scans the given files for music metadata.
        /// </summary>
        /// <param name="filesToScan">The files that will be scanned for metadata. Invalid or unsupported files will be skipped.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is all discovered metadata from the scanned files.</returns>
        public Task<IEnumerable<FileMetadata>> ScanMusicFiles(IEnumerable<IFileData> filesToScan)
        {
            return ScanMusicFilesAsync(filesToScan, new CancellationToken());
        }

        /// <summary>
        /// Scans the given files for music metadata.
        /// </summary>
        /// <param name="filesToScan">The files that will be scanned for metadata. Invalid or unsupported files will be skipped.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that will cancel the scanning task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is all discovered metadata from the scanned files.</returns>
        public async Task<IEnumerable<FileMetadata>> ScanMusicFilesAsync(IEnumerable<IFileData> filesToScan, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"{nameof(ScanMusicFilesAsync)} started");

            _filesProcessed = 0;

            if (cancellationToken.IsCancellationRequested)
                cancellationToken.ThrowIfCancellationRequested();

            _scanningCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var musicFiles = filesToScan.Where(x => _supportedMusicFileFormats.Contains(x.FileExtension));
            var remainingFilesToScan = new Queue<IFileData>(musicFiles);

            _filesToScanCount = remainingFilesToScan.Count;

            _metadataManager.FilesFound = _filesToScanCount;

            try
            {
                Guard.HasSizeGreaterThan(remainingFilesToScan, 0, nameof(remainingFilesToScan));

                _logger.LogInformation($"{nameof(ScanMusicFilesAsync)}: Queued processing of {remainingFilesToScan.Count} files.");

                while (remainingFilesToScan.Count > 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    var batchSize = _scanBatchSize;

                    // Prevent going out of range
                    if (batchSize > remainingFilesToScan.Count)
                        batchSize = remainingFilesToScan.Count;

                    // Pull assets out of the queue to create a batch
                    var currentBatch = new IFileData[batchSize];
                    for (var i = 0; i < batchSize; i++)
                    {
                        currentBatch[i] = remainingFilesToScan.Dequeue();
                    }

                    // Scan the files in the current batch
                    _logger.LogInformation($"{nameof(ScanMusicFilesAsync)}: Starting batch processing of {batchSize} files. ({remainingFilesToScan.Count} remaining)");
                    await Task.Run(() => currentBatch.InParallel(ProcessFile), cancellationToken);
                }

                return _allFileMetadata;
            }
            catch (OperationCanceledException)
            {
                _scanningCancellationTokenSource.Dispose();
                return new List<FileMetadata>();
            }
        }

        private static void AssignMissingRequiredData(IFileData fileData, FileMetadata metadata)
        {
            // If titles are missing, we leave it empty so the UI can localize the "Untitled" name.
            metadata.Id = fileData.Path.HashMD5Fast();

            Guard.IsNotNullOrWhiteSpace(metadata.Id, nameof(metadata.Id));
            Guard.IsNotNull(metadata.TrackMetadata, nameof(metadata.TrackMetadata));
            Guard.IsNotNull(metadata.AlbumMetadata, nameof(metadata.AlbumMetadata));
            Guard.IsNotNull(metadata.ArtistMetadata, nameof(metadata.ArtistMetadata));

            // Track
            if (string.IsNullOrWhiteSpace(metadata.TrackMetadata.Title))
                metadata.TrackMetadata.Title = string.Empty;

            metadata.TrackMetadata.Id ??= metadata.Id;

            metadata.TrackMetadata.ArtistIds ??= new HashSet<string>();
            metadata.TrackMetadata.ImageIds ??= new HashSet<string>();

            // Album
            if (string.IsNullOrWhiteSpace(metadata.AlbumMetadata.Title))
                metadata.AlbumMetadata.Title = string.Empty;

            var albumId = (metadata.AlbumMetadata.Title + "_album").HashMD5Fast();
            metadata.AlbumMetadata.Id = albumId;

            metadata.AlbumMetadata.ArtistIds ??= new HashSet<string>();
            metadata.AlbumMetadata.ImageIds ??= new HashSet<string>();
            metadata.AlbumMetadata.TrackIds ??= new HashSet<string>();

            // Artist
            if (string.IsNullOrWhiteSpace(metadata.ArtistMetadata.Name))
                metadata.ArtistMetadata.Name = string.Empty;

            var artistId = (metadata.ArtistMetadata.Name + "_artist").HashMD5Fast();
            metadata.ArtistMetadata.Id = artistId;

            metadata.ArtistMetadata.AlbumIds ??= new HashSet<string>();
            metadata.ArtistMetadata.TrackIds ??= new HashSet<string>();
            metadata.ArtistMetadata.ImageIds ??= new HashSet<string>();

            Guard.IsNotNullOrWhiteSpace(metadata.TrackMetadata.Id, nameof(metadata.TrackMetadata.Id));
            Guard.IsNotNullOrWhiteSpace(metadata.AlbumMetadata.Id, nameof(metadata.AlbumMetadata.Id));
            Guard.IsNotNullOrWhiteSpace(metadata.ArtistMetadata.Id, nameof(metadata.ArtistMetadata.Id));
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

        private void LinkMetadataIdsForFile(FileMetadata metadata)
        {
            // Each fileMetadata is the data for a single file.
            // Album and artist ID are generated based on Title/Name
            // so blindly linking based on Ids found in a single file is safe.

            // The list of IDs for, e.g., the tracks in an AlbumMetadata, are merged by the repositories.
            Guard.IsNotNullOrWhiteSpace(metadata.AlbumMetadata?.Id, nameof(metadata.AlbumMetadata.Id));
            Guard.IsNotNullOrWhiteSpace(metadata.ArtistMetadata?.Id, nameof(metadata.ArtistMetadata.Id));
            Guard.IsNotNullOrWhiteSpace(metadata.TrackMetadata?.Id, nameof(metadata.TrackMetadata.Id));
            Guard.IsNotNull(metadata.TrackMetadata?.Url, nameof(metadata.TrackMetadata.Url));

            _logger.LogInformation($"Cross-linking IDs for metadata ID {metadata.Id} located at {metadata.TrackMetadata.Url}");

            // Albums
            Guard.IsNotNull(metadata.AlbumMetadata?.ArtistIds, nameof(metadata.AlbumMetadata.ArtistIds));
            Guard.IsNotNull(metadata.AlbumMetadata?.TrackIds, nameof(metadata.AlbumMetadata.TrackIds));

            if (!metadata.AlbumMetadata.ArtistIds.Contains(metadata.AlbumMetadata.Id))
                metadata.AlbumMetadata.ArtistIds.Add(metadata.ArtistMetadata.Id);

            if (!metadata.AlbumMetadata.TrackIds.Contains(metadata.TrackMetadata.Id))
                metadata.AlbumMetadata.TrackIds.Add(metadata.TrackMetadata.Id);

            // Artists
            Guard.IsNotNull(metadata.ArtistMetadata?.TrackIds, nameof(metadata.ArtistMetadata.TrackIds));
            Guard.IsNotNull(metadata.ArtistMetadata?.AlbumIds, nameof(metadata.ArtistMetadata.AlbumIds));

            if (!metadata.ArtistMetadata.TrackIds.Contains(metadata.TrackMetadata.Id))
                metadata.ArtistMetadata.TrackIds.Add(metadata.TrackMetadata.Id);

            if (!metadata.ArtistMetadata.AlbumIds.Contains(metadata.AlbumMetadata.Id))
                metadata.ArtistMetadata.AlbumIds.Add(metadata.AlbumMetadata.Id);

            // Tracks
            Guard.IsNotNull(metadata.TrackMetadata?.ArtistIds, nameof(metadata.TrackMetadata.ArtistIds));

            if (!metadata.TrackMetadata.ArtistIds.Contains(metadata.ArtistMetadata.Id))
                metadata.TrackMetadata.ArtistIds.Add(metadata.ArtistMetadata.Id);

            metadata.TrackMetadata.AlbumId = metadata.AlbumMetadata.Id;
        }

        private async Task<FileMetadata?> GetMusicFilesProperties(IFileData fileData)
        {
            _logger.LogInformation($"{nameof(GetMusicFilesProperties)} entered for {nameof(IFileData)} at {fileData.Path}");

            var details = await fileData.Properties.GetMusicPropertiesAsync();

            Stream? imageStream = null;

            imageStream = await fileData.GetThumbnailAsync(ThumbnailMode.MusicView, 256);

            if (details is null)
                return null;

            var relatedMetadata = new FileMetadata
            {
                AlbumMetadata = new AlbumMetadata
                {
                    Title = details.Album,
                    Duration = details.Duration,
                    Genres = new HashSet<string>(details.Genres?.PruneNull()),
                },
                TrackMetadata = new TrackMetadata
                {
                    TrackNumber = details.TrackNumber,
                    Title = details.Title,
                    Genres = new HashSet<string>(details.Genres?.PruneNull()),
                    Duration = details.Duration,
                    Url = fileData.Path,
                    Year = details.Year,
                },
                ArtistMetadata = new ArtistMetadata
                {
                    Genres = new HashSet<string>(details.Genres?.PruneNull()),
                    Name = details.Artist,
                },
            };

            if (imageStream != null && imageStream.Length > 0)
            {
                Guard.IsNotNull(_scanningCancellationTokenSource, nameof(_scanningCancellationTokenSource));

                var stream = new List<Stream>() { imageStream };
                Task.Run(() => ProcessImagesAsync(fileData, relatedMetadata, stream), _scanningCancellationTokenSource.Token).Forget();
            }

            return relatedMetadata;
        }

        private async Task<FileMetadata?> ScanFileMetadata(IFileData fileData)
        {
            var foundMetadata = new List<FileMetadata>();

            if (_metadataManager.ScanTypes.HasFlag(MetadataScanTypes.TagLib))
            {
                var id3Metadata = await GetId3Metadata(fileData);

                if (!(id3Metadata is null))
                    foundMetadata.Add(id3Metadata);
            }

            if (_metadataManager.ScanTypes.HasFlag(MetadataScanTypes.FileProperties))
            {
                var propertyMetadata = await GetMusicFilesProperties(fileData);

                if (!(propertyMetadata is null))
                    foundMetadata.Add(propertyMetadata);
            }

            var validMetadata = foundMetadata.ToArray();
            if (validMetadata.Length == 0)
                return null;

            var aggregatedData = MergeMetadataFields(validMetadata);

            // Assign missing titles and IDs
            AssignMissingRequiredData(fileData, aggregatedData);

            LinkMetadataIdsForFile(aggregatedData);

            return aggregatedData;
        }

        private async Task<FileMetadata?> GetId3Metadata(IFileData fileData)
        {
            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));
            Guard.IsNotNull(_scanningCancellationTokenSource, nameof(_scanningCancellationTokenSource));

            _logger.LogInformation($"{nameof(GetId3Metadata)} entered for {nameof(IFileData)} at {fileData.Path}");

            try
            {
                using var stream = await fileData.GetStreamAsync(FileAccessMode.ReadWrite);

                // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                // Some underlying libs without nullable checks may return null by mistake.
                if (stream is null)
                    return null;

                stream.Seek(0, SeekOrigin.Begin);

                TagLibHelper.TryAddManualFileTypeResolver();

                _logger.LogInformation($"Creating {nameof(TagLib.File)} instance.");

                try
                {
                    using var tagFile = TagLib.File.Create(new FileAbstraction(fileData.Name, stream), ReadStyle.Average);
                    var tag = tagFile.Tag;

                    // If there's no metadata to read, return null
                    if (tag == null)
                    {
                        _logger.LogInformation($"{nameof(IFileData)} at {fileData.Path}: no metadata found.");
                        return null;
                    }

                    var fileMetadata = new FileMetadata
                    {
                        AlbumMetadata = new AlbumMetadata
                        {
                            Description = tag.Description,
                            Title = tag.Album,
                            Duration = tagFile.Properties.Duration,
                            Genres = new HashSet<string>(tag.Genres),
                            DatePublished = tag.DateTagged,
                            ArtistIds = new HashSet<string>(),
                            TrackIds = new HashSet<string>(),
                            ImageIds = new HashSet<string>(),
                        },
                        TrackMetadata = new TrackMetadata
                        {
                            Url = fileData.Path,
                            Description = tag.Description,
                            Title = tag.Title,
                            DiscNumber = tag.Disc,
                            Duration = tagFile.Properties.Duration,
                            Genres = new HashSet<string>(tag.Genres),
                            TrackNumber = tag.Track,
                            Year = tag.Year,
                            ArtistIds = new HashSet<string>(),
                            ImageIds = new HashSet<string>(),
                        },
                        ArtistMetadata = new ArtistMetadata
                        {
                            Name = tag.FirstAlbumArtist,
                            Genres = new HashSet<string>(tag.Genres),
                            AlbumIds = new HashSet<string>(),
                            TrackIds = new HashSet<string>(),
                            ImageIds = new HashSet<string>(),
                        },
                    };

                    if (tag.Pictures != null)
                    {
                        _logger.LogInformation($"{nameof(IFileData)} at {fileData.Path}: Images found");
                        var imageStreams = tag.Pictures.Select(x => x.Data.Data).Select(x => new MemoryStream(x));

                        Task.Run(() => ProcessImagesAsync(fileData, fileMetadata, imageStreams), _scanningCancellationTokenSource.Token).Forget();
                    }

                    _logger.LogInformation($"{nameof(IFileData)} at {fileData.Path}: Metadata scan completed.");
                    return fileMetadata;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{ex}");
                    return null;
                }
            }
            catch (CorruptFileException ex)
            {
                _logger.LogError(ex, $"{nameof(CorruptFileException)} for {nameof(IFileData)} at {fileData.Path}");
                return null;
            }
            catch (UnsupportedFormatException ex)
            {
                _logger.LogError(ex, $"{nameof(UnsupportedFormatException)} for {nameof(IFileData)} at {fileData.Path}");
                return null;
            }
            catch (FileLoadException ex)
            {
                _logger.LogError(ex, $"{nameof(FileLoadException)} for {nameof(IFileData)} at {fileData.Path}");
                return null;
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogError(ex, $"{nameof(FileNotFoundException)} for {nameof(IFileData)} at {fileData.Path}");
                return null;
            }
            catch (ArgumentException ex)
            {
                _logger.LogError(ex, $"{nameof(ArgumentException)} for {nameof(IFileData)} at {fileData.Path}");
                return null;
            }
        }

        private async Task<FileMetadata?> ProcessFile(IFileData file)
        {
            var fileMetadata = await ScanFileMetadata(file);

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            _metadataManager.FilesProcessed = ++_filesProcessed;

            await _batchLock.WaitAsync();

            if (fileMetadata != null)
            {
                _allFileMetadata.Add(fileMetadata);
                _batchMetadataToEmit.Add(fileMetadata);
            }
            else
            {
                _logger.Log(LogLevel.Warning, $"{nameof(ProcessFile)}: file scan return no metadata and will be ignored. (at {file.Path})");
            }

            _batchLock.Release();

            _ = HandleChangedAsync();

            return fileMetadata;
        }

        private async Task HandleChangedAsync()
        {
            if (!await Flow.Debounce(_emitDebouncerId, TimeSpan.FromSeconds(1)))
                return;

            await _batchLock.WaitAsync();

            if (_batchMetadataToEmit.Count == 0)
            {
                _batchLock.Release();
                return;
            }

            bool IsEnoughMetadataToEmit() => _batchMetadataToEmit.Count >= 75;
            bool IsFinishedScanning() => _filesProcessed == _filesToScanCount;

            if (!(IsFinishedScanning() || IsEnoughMetadataToEmit()))
            {
                _batchLock.Release();
                return;
            }

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            _logger.LogInformation($"{nameof(HandleChangedAsync)}: Emitting {_batchMetadataToEmit.Count} scanned items.");

            FileMetadataAdded?.Invoke(this, _batchMetadataToEmit.ToArray());

            _batchMetadataToEmit.Clear();
            _batchLock.Release();

            if (IsFinishedScanning())
            {
                _logger.LogInformation($"{nameof(HandleChangedAsync)}: finished scanning.");
                FileScanCompleted?.Invoke(this, _allFileMetadata);
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            DetachEvents();
        }
    }
}
