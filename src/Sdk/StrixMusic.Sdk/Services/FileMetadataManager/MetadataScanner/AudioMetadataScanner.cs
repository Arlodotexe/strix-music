using Microsoft.Toolkit.Diagnostics;
using OwlCore;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
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
    /// Handles scanning of individual audio files for metadata.
    /// </summary>
    public class AudioMetadataScanner : IDisposable
    {
        private const int SCAN_BATCH_SIZE = 2;
        private static readonly string[] _supportedMusicFileFormats = { ".mp3", ".flac", ".m4a", ".wma" };
        private static readonly IReadOnlyList<int> _standardImageSizes = new int[] { 64, 128, 256, 512, 1024 };

        private readonly FileMetadataManager _metadataManager;
        private readonly IFileScanner _fileScanner;
        private readonly SemaphoreSlim _batchLock;
        private readonly SemaphoreSlim _imageBatchLock;

        private readonly string _emitDebouncerId = Guid.NewGuid().ToString();
        private readonly string _emitImagesDebouncerId = Guid.NewGuid().ToString();
        private readonly HashSet<FileMetadata> _batchMetadataToEmit = new HashSet<FileMetadata>();
        private readonly HashSet<ImageMetadata> _batchImageMetadataToEmit = new HashSet<ImageMetadata>();
        private readonly HashSet<FileMetadata> _allFileMetadata = new HashSet<FileMetadata>();

        private CancellationTokenSource? _scanningCancellationTokenSource;
        private int _filesToScanCount;
        private int _filesProcessed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AudioMetadataScanner"/> class.
        /// </summary>
        /// <param name="fileScanner">The instance of <see cref="DepthFirstFileScanner"/> that will discover files to scan.</param>
        /// <param name="metadataManager">The metadata manager that handles this scanner.</param>       
        public AudioMetadataScanner(IFileScanner fileScanner, FileMetadataManager metadataManager)
        {
            _metadataManager = metadataManager;
            _fileScanner = fileScanner;

            _batchLock = new SemaphoreSlim(1, 1);
            _imageBatchLock = new SemaphoreSlim(1, 1);

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
        public event EventHandler<IEnumerable<FileMetadata>>? FileMetadataRemoved;

        /// <summary>
        /// Raised when all file scanning is complete.
        /// </summary>
        public event EventHandler<IEnumerable<FileMetadata>>? FileScanCompleted;

        /// <summary>
        /// Raised when images from files are discovered/resized.
        /// </summary>
        public event EventHandler<IEnumerable<ImageMetadata>>? ImageMetadataAdded;

        /// <inheritdoc/>
        public bool IsInitialized { get; set; }

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
            return ScanMusicFiles(filesToScan, new CancellationToken());
        }

        /// <summary>
        /// Scans the given files for music metadata.
        /// </summary>
        /// <param name="filesToScan">The files that will be scanned for metadata. Invalid or unsupported files will be skipped.</param>
        /// <param name="cancellationToken">A <see cref="CancellationToken"/> that will cancel the scanning task.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation. Value is all discovered metadata from the scanned files.</returns>
        public async Task<IEnumerable<FileMetadata>> ScanMusicFiles(IEnumerable<IFileData> filesToScan, CancellationToken cancellationToken)
        {
            _scanningCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            var musicFiles = filesToScan.Where(x => _supportedMusicFileFormats.Contains(x.FileExtension));
            var remainingFilesToScan = new Queue<IFileData>(musicFiles);

            _filesToScanCount = remainingFilesToScan.Count;

            _metadataManager.FilesFound = _filesToScanCount;

            try
            {
                Guard.HasSizeGreaterThan(remainingFilesToScan, 0, nameof(remainingFilesToScan));

                while (remainingFilesToScan.Count > 0)
                {
                    if (cancellationToken.IsCancellationRequested)
                        cancellationToken.ThrowIfCancellationRequested();

                    var batchSize = SCAN_BATCH_SIZE;

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
                    await Task.Run(() => currentBatch.InParallel(ProcessFile));
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

                if (metadata.TrackMetadata.ImageIds is null)
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

        private static async Task<FileMetadata?> GetMusicFilesProperties(IFileData fileData)
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

        private async Task<FileMetadata?> GetId3Metadata(IFileData fileData)
        {
            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));

            try
            {
                using var stream = await fileData.GetStreamAsync();

                using var tagFile = File.Create(new FileAbstraction(fileData.Name, stream), ReadStyle.Average);

                // Read the raw tags
                var tags = tagFile.GetTag(TagTypes.Id3v2) ??
                           tagFile.GetTag(TagTypes.Asf) ??
                           tagFile.GetTag(TagTypes.FlacMetadata);

                // If there's no metadata to read, return null
                if (tags == null)
                    return null;

                if (tags.Pictures != null)
                    _ = HandleImagesAsync(fileData.Path, tags.Pictures);
                
                return new FileMetadata
                {
                    AlbumMetadata = new AlbumMetadata
                    {
                        Description = tags.Description,
                        Title = tags.Album,
                        ImageIds = new List<string>(),
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        DatePublished = tags.DateTagged,
                        ArtistIds = new List<string>(),
                        TrackIds = new List<string>(),
                    },
                    TrackMetadata = new TrackMetadata
                    {
                        Source = new Uri(fileData.Path),
                        ImageIds = new List<string>(),
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
                        ImageIds = new List<string>(),
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

        private async Task HandleImagesAsync(string filePath, IPicture[] pictures)
        {
            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));

            // We store images for a file in the following structure:
            // CacheFolder/Images/{hashed file path}/{image ID}-{size}.png
            var baseImagesFolder = await CacheFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
            var fileImagesFolder = await baseImagesFolder.CreateFolderAsync(filePath.HashMD5Fast(), CreationCollisionOption.OpenIfExists);

            foreach (var picture in pictures)
            {
                try
                {
                    var imageData = picture.Data.Data;

                    // Create a unique ID for the image by collecting every 16th byte in the data
                    // and calculating an MD5 hash for it.
                    // This will form the base file name for the image.
                    // Each resized version will use this name with its size appended.
                    var hashData = new byte[imageData.Length / 16];
                    for (var i = 0; i < hashData.Length; i++)
                    {
                        hashData[i] = imageData[i * 16];
                    }

                    var imageId = hashData.HashMD5Fast();

                    using var imageStream = new MemoryStream(imageData);
                    using var image = await Image.LoadAsync(imageStream);

                    // We don't want to scale the image up (only scale down if necessary), so we have to determine which of the
                    // standard image sizes we have to resize to using the size of the original image.

                    // Use the maximum of the width and height for the ceiling to handle cases where the image isn't a 1:1 aspect ratio.
                    var ceiling = Math.Max(image.Width, image.Height);

                    // Loop through the standard image sizes (in ascending order) and determine the maximum size
                    // that the original image is larger than. We'll scale it down to that size and all the sizes smaller than it.
                    var useOriginal = false;
                    var ceilingSizeIndex = -1;
                    for (var i = 0; i < _standardImageSizes.Count; i++)
                    {
                        if (ceiling > _standardImageSizes[i])
                        {
                            ceilingSizeIndex = i;
                        }
                        else
                        {
                            if (ceiling == _standardImageSizes[i])
						    {
                                // If the size of the image is equal to one of the standard image sizes,
                                // we can skip the expensive resize operation and just copy the original image.
                                useOriginal = true;
						    }

                            break;
                        }
                    }

                    var metadata = new List<ImageMetadata>();

                    // If the ceiling size index is -1, the image is smaller than all of the standard image sizes.
                    // In this case, we'll just use the original image size and copy the image into the cache folder
                    // rather than resizing it.
                    // We can also use this same logic when useOriginal is true.
                    if (ceilingSizeIndex == -1 || useOriginal)
                    {
                        var fullId = $"{imageId}-{ceiling}";
                        var imageFile = await fileImagesFolder.CreateFileAsync($"{fullId}.png", CreationCollisionOption.ReplaceExisting);
                        using var stream = await imageFile.GetStreamAsync(FileAccessMode.ReadWrite);
                        await image.SaveAsPngAsync(stream);

                        metadata.Add(new ImageMetadata
                        {
                            Id = fullId,
                            Uri = new Uri(imageFile.Path),
                            Width = image.Width,
                            Height = image.Height
                        });
                    }

                    if (ceilingSizeIndex != -1)
					{
                        for (var i = ceilingSizeIndex; i >= 0; i--)
						{
                            var resizedSize = _standardImageSizes[i];
                            var resizedWidth = image.Width > image.Height ? 0 : resizedSize;
                            var resizedHeight = image.Height > image.Width ? 0 : resizedSize;

                            var fullId = $"{imageId}-{resizedSize}";
                            var imageFile = await fileImagesFolder.CreateFileAsync($"{fullId}.png", CreationCollisionOption.ReplaceExisting);
                            using var stream = await imageFile.GetStreamAsync(FileAccessMode.ReadWrite);

                            image.Mutate(x => x.Resize(resizedWidth, resizedHeight));

                            await image.SaveAsPngAsync(stream);

                            metadata.Add(new ImageMetadata
                            {
                                Id = fullId,
                                Uri = new Uri(imageFile.Path),
                                Width = image.Width,
                                Height = image.Height
                            });
                        }
					}

                    await _imageBatchLock.WaitAsync();
                    foreach (var m in metadata)
                        _batchImageMetadataToEmit.Add(m);
                    _imageBatchLock.Release();

                    _ = HandleImagesChangedAsync();
                }
                catch (FileLoadException)
                {
                    // Catch "The file is in use." error.
                }
                catch (UnknownImageFormatException)
				{
                    // TODO: Handle this better? Might be easier to just skip unsupported image formats.
                    // Perhaps just filter them out before calling this method?
				}
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

            _batchLock.Release();

            _ = HandleChanged();

            return fileMetadata;
        }

        private async Task HandleChanged()
        {
            if (!await Flow.Debounce(_emitDebouncerId, TimeSpan.FromSeconds(1)))
                return;

            await _batchLock.WaitAsync();

            if (_batchMetadataToEmit.Count == 0)
            {
                _batchLock.Release();
                return;
            }

            bool IsEnoughMetadataToEmit() => _batchMetadataToEmit.Count >= 100;
            bool IsFinishedScanning() => _filesProcessed == _filesToScanCount;

            if (!IsFinishedScanning() && !IsEnoughMetadataToEmit())
            {
                _batchLock.Release();
                return;
            }

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            FileMetadataAdded?.Invoke(this, _batchMetadataToEmit.ToArray());

            _batchMetadataToEmit.Clear();
            _batchLock.Release();

            if (IsFinishedScanning())
                FileScanCompleted?.Invoke(this, _allFileMetadata);
        }

        private async Task HandleImagesChangedAsync()
        {
            if (!await Flow.Debounce(_emitImagesDebouncerId, TimeSpan.FromSeconds(1)))
                return;

            await _imageBatchLock.WaitAsync();

            if (_batchImageMetadataToEmit.Count == 0)
            {
                _imageBatchLock.Release();
                return;
            }

            bool IsEnoughMetadataToEmit() => _batchImageMetadataToEmit.Count >= 100;
            bool IsFinishedScanning() => _filesProcessed == _filesToScanCount;

            if (!IsFinishedScanning() && !IsEnoughMetadataToEmit())
            {
                _imageBatchLock.Release();
                return;
            }

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            ImageMetadataAdded?.Invoke(this, _batchImageMetadataToEmit.ToArray());

            _batchImageMetadataToEmit.Clear();
            _imageBatchLock.Release();
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

                _scanningCancellationTokenSource?.Cancel();
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
        ~AudioMetadataScanner()
        {
            Dispose(false);
        }
    }
}
