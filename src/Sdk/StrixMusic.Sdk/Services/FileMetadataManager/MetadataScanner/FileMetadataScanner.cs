using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OwlCore.AbstractStorage;
using OwlCore.AbstractUI.Models;
using OwlCore.Extensions;
using OwlCore.Provisos;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using TagLib;
using File = TagLib.File;

namespace StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner
{
    /// <summary>
    /// Handles scanning tracks for all supported metadata, including notification support for when data changes.
    /// </summary>
    public class FileMetadataScanner : IAsyncInit, IDisposable
    {
        private readonly ConcurrentBag<FileMetadata> _fileMetadata = new ConcurrentBag<FileMetadata>();
        private readonly ConcurrentBag<IFileData> _unscannedFiles = new ConcurrentBag<IFileData>();
        private readonly ConcurrentBag<IFolderData> _unscannedFolders = new ConcurrentBag<IFolderData>();
        private readonly IFolderData _folderData;
        private TaskCompletionSource<List<FileMetadata>>? _folderScanningTaskCompletion;
        private SynchronizationContext _sync;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="FileMetadataScanner"/>.
        /// </summary>
        /// <param name="rootFolder">The root folder to operate in when scanning. Will be scanned recursively.</param>
        public FileMetadataScanner(IFolderData rootFolder)
        {
            _folderData = rootFolder;

            _sync = SynchronizationContext.Current;

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
            try
            {
                var stream = await fileData.GetStreamAsync();

                var prevSync = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(_sync);

                using var tagFile = File.Create(new FileAbstraction(fileData.Name, stream), ReadStyle.Average);

                SynchronizationContext.SetSynchronizationContext(prevSync);

                // Read the raw tags
                var tags = tagFile.GetTag(TagTypes.Id3v2);

                // If there's no metadata to read, return null
                if (tags == null)
                    return null;

                if (tags.Pictures != null && tags.Pictures.Length > 0)
                {
                    var albumArt = tags.Pictures.First(p => p.Type == PictureType.FrontCover);
                    string filename = albumArt.Filename;
                    byte[] imageData = albumArt.Data.Data;
                    
                }

                return new FileMetadata
                {
                    AlbumMetadata = new AlbumMetadata()
                    {
                        Description = tags.Description,
                        Title = tags.Album,
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        DatePublished = tags.DateTagged,
                        TotalTracksCount = Convert.ToInt32(tags.TrackCount),
                        TotalArtistsCount = tags.AlbumArtists.Length,
                    },

                    TrackMetadata = new TrackMetadata()
                    {
                        Source = new Uri(fileData.Path),
                        Description = tags.Description,
                        Title = tags.Title,
                        DiscNumber = tags.Disc,
                        Duration = tagFile.Properties.Duration,
                        Genres = new List<string>(tags.Genres),
                        TrackNumber = tags.Track,
                        Year = tags.Year,
                    },
                    ArtistMetadata = new ArtistMetadata()
                    {
                        Name = tags.FirstAlbumArtist,
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
        private void ApplyRelatedMetadataIds(ConcurrentBag<FileMetadata> relatedMetadata)
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

            // todo check and throw if metadata isn't scanned
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

            await ScanFolderForMetadata();

            IsInitialized = true;
        }

        /// <summary>
        /// Scans a folder and all subfolders for music and music metadata.
        /// </summary>
        /// <returns>Fully scanned <see cref="IReadOnlyList{RelatedMetadata}"/>.</returns>
        private async Task<List<FileMetadata>> ScanFolderForMetadata()
        {
            _folderScanningTaskCompletion = new TaskCompletionSource<List<FileMetadata>>();

            // Queue initial items from root folder on main thread.
            _unscannedFolders.Add(_folderData);

            // Create a thread for each processor.
            int threadCount = Environment.ProcessorCount;
            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(RunThreadLoop);
                thread.Name = $"SC Thr #{i}";
                thread.Start();
            }

            var result = _fileMetadata.ToList();

            _folderScanningTaskCompletion?.SetResult(result);
            _folderScanningTaskCompletion = null;

            return result;
        }

        /// <summary>
        /// Runs a thread to scan for or process files.
        /// </summary>
        private async void RunThreadLoop()
        {
            bool doneWork = false;
            while (true)
            {
                if (_unscannedFiles.TryTake(out IFileData file))
                {
                    await ProcessFile(file);
                    doneWork = true;
                }
                else if (_unscannedFolders.TryTake(out IFolderData folder))
                {
                    await QueueItemsFromFolder(folder);
                    doneWork = true;
                }
                else if (doneWork)
                {
                    continue;
                }
            }
        }

        /// <summary>
        /// Finds all storage items in a folder and adds them to their unscanned bags.
        /// </summary>
        /// <param name="folder">The folder to iterate for items.</param>
        private async Task QueueItemsFromFolder(IFolderData folder)
        {
            Debug.WriteLine($"Thread {Thread.CurrentThread.Name} scanning for items in: {folder.Path}");
            var subfolders = await folder.GetFoldersAsync();
            var files = await folder.GetFilesAsync();
            foreach (var subfolder in subfolders)
            {
                _unscannedFolders.Add(subfolder);
            }

            foreach (var file in files)
            {
                _unscannedFiles.Add(file);
            }
        }

        private async Task ProcessFile(IFileData file)
        {
            Debug.WriteLine($"Thread {Thread.CurrentThread.Name} processing file: {file.Path}");

            var fileMetadata = await ScanFileMetadata(file);

            if (fileMetadata == null)
                return;

            _fileMetadata.Add(fileMetadata);
            ApplyRelatedMetadataIds(_fileMetadata);

            FileMetadataAdded?.Invoke(this, fileMetadata);
        }
    }
}
