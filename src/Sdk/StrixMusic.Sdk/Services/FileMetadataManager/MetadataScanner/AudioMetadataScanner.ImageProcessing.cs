using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.AbstractStorage;
using OwlCore.Extensions;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using StrixMusic.Sdk.Services.FileMetadataManager.Models;
using ImageMetadata = StrixMusic.Sdk.Services.FileMetadataManager.Models.ImageMetadata;

namespace StrixMusic.Sdk.Services.FileMetadataManager.MetadataScanner
{
    public partial class AudioMetadataScanner
    {
        private static readonly IReadOnlyList<int> _standardImageSizes = new int[] { 64, 128, 256, 512 };
        private readonly ConcurrentDictionary<string, Task<IEnumerable<ImageMetadata>>> _ongoingImageProcessingTasks;
        private readonly SemaphoreSlim _ongoingImageProcessingTasksSemaphore;
        private readonly SemaphoreSlim _ongoingImageProcessingSemaphore;

        private static string GenerateImageId(Stream imageStream)
        {
            // Create a unique ID for the image by collecting every 32nd byte in the data
            // and calculating a rough hash for it.
            // This will form the base file name for the image.
            // Each resized version will use this name with its size appended.
            var hashData = new byte[imageStream.Length / 32];

            imageStream.Position = 0;

            for (var i = 0; i < hashData.Length; i++)
            {
                imageStream.Position = i * 32;

                hashData[i] = (byte)imageStream.ReadByte();
            }

            return hashData.HashMD5Fast();
        }

        private async Task ProcessImagesAsync(IFileData fileData, FileMetadata fileMetadata, IEnumerable<Stream> imageStreams)
        {
            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            var results = new List<ImageMetadata>();

            await imageStreams.InParallel(async image =>
            {
                // Image IDs are unique to the content of the image.
                var imageId = GenerateImageId(image);

                // Check if the data has already been emitted.
                await _batchLock.WaitAsync();

                var foundData = false;
                foreach (var metadata in _allFileMetadata)
                {
                    if (metadata.ImageMetadata is null)
                        continue;

                    foreach (var imageMetadata in metadata.ImageMetadata)
                    {
                        if (imageMetadata.Id == imageId)
                        {
                            results.Add(imageMetadata);
                            foundData = true;
                        }
                    }
                }

                _batchLock.Release();

                if (foundData)
                {
                    image.Dispose();
                    return;
                }

                // Check if this image is still processing
                await _ongoingImageProcessingTasksSemaphore.WaitAsync();

                if (_ongoingImageProcessingTasks.TryGetValue(imageId, out var ongoingTask))
                {
                    _ongoingImageProcessingTasksSemaphore.Release();
                    var relevantImages = await ongoingTask;
                    results.AddRange(relevantImages);
                    image.Dispose();
                    return;
                }

                // Start processing and add to ongoing tasks.
                var processImageTask = ProcessImageAsync(image);

                if (!_ongoingImageProcessingTasks.TryAdd(imageId, processImageTask))
                    ThrowHelper.ThrowInvalidOperationException($"Unable to add new task to {nameof(_ongoingImageProcessingTasks)}");

                _ongoingImageProcessingTasksSemaphore.Release();

                var imageScanResult = await processImageTask;
                image.Dispose();

                await _ongoingImageProcessingTasksSemaphore.WaitAsync();

                if (!_ongoingImageProcessingTasks.TryRemove(imageId, out _))
                    ThrowHelper.ThrowInvalidOperationException($"Unable to remove finished task from {nameof(_ongoingImageProcessingTasks)}");

                results.AddRange(imageScanResult);

                _ongoingImageProcessingTasksSemaphore.Release();
            });

            await _batchLock.WaitAsync();

            fileMetadata.ImageMetadata = results;

            Guard.IsNotNull(fileMetadata.AlbumMetadata, nameof(fileMetadata.AlbumMetadata));

            // Must create a new instance to make an update.
            // FileCoreAlbum holds a reference to any emitted data, meaning it sees any changes to IDs before the data is in the repo.
            var updatedAlbumMetadata = new AlbumMetadata
            {
                Id = fileMetadata.AlbumMetadata.Id,
                Title = fileMetadata.AlbumMetadata.Title,
                Description = fileMetadata.AlbumMetadata.Description,
                TrackIds = fileMetadata.AlbumMetadata.TrackIds,
                ArtistIds = fileMetadata.AlbumMetadata.ArtistIds,
                Duration = fileMetadata.AlbumMetadata.Duration,
                DatePublished = fileMetadata.AlbumMetadata.DatePublished,
                Genres = fileMetadata.AlbumMetadata.Genres,
                ImageIds = new HashSet<string>(results.Select(x => x.Id).PruneNull()),
            };

            fileMetadata.AlbumMetadata = updatedAlbumMetadata;

            AssignMissingRequiredData(fileData, fileMetadata);
            LinkMetadataIdsForFile(fileMetadata);

            // Re-emit as an update if not already queued.
            _allFileMetadata.Add(fileMetadata);
            _batchMetadataToEmit.Add(fileMetadata);

            _batchLock.Release();

            _ = HandleChangedAsync();
        }

        private async Task<IEnumerable<ImageMetadata>> ProcessImageAsync(Stream imageStream)
        {
            await _ongoingImageProcessingSemaphore.WaitAsync();

            Guard.IsNotNull(CacheFolder, nameof(CacheFolder));

            // We store images for a file in the following structure:
            // CacheFolder/Images/{image ID}-{size}.png
            // image ID is calculated based on content. Using a shared folder means no duplicate images.
            var fileImagesFolder = await CacheFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

            if (_scanningCancellationTokenSource?.Token.IsCancellationRequested ?? false)
                _scanningCancellationTokenSource?.Token.ThrowIfCancellationRequested();

            Image? img = null;

            try
            {
                imageStream.Position = 0;
                img = await Image.LoadAsync(imageStream);
            }
            catch (UnknownImageFormatException)
            {
                // TODO: Handle this better? Might be easier to just skip unsupported image formats.
                // Perhaps just filter them out before calling this method?
                _ongoingImageProcessingSemaphore.Release();
                return Enumerable.Empty<ImageMetadata>();
            }

            using var image = img;

            var imageId = GenerateImageId(imageStream);
            imageStream.Position = 0;

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
                await stream.FlushAsync();

                _ongoingImageProcessingSemaphore.Release();
                return new ImageMetadata
                {
                    Id = fullId,
                    Uri = new Uri(imageFile.Path),
                    Width = image.Width,
                    Height = image.Height
                }.IntoList();
            }

            var results = new List<ImageMetadata>();

            for (var i = ceilingSizeIndex; i >= 0; i--)
            {
                var resizedSize = _standardImageSizes[i];
                var resizedWidth = image.Width > image.Height ? 0 : resizedSize;
                var resizedHeight = image.Height > image.Width ? 0 : resizedSize;

                var fullId = $"{imageId}-{resizedSize}";
                var imageFile = await fileImagesFolder.CreateFileAsync($"{fullId}.png", CreationCollisionOption.ReplaceExisting);
                var stream = await imageFile.GetStreamAsync(FileAccessMode.ReadWrite);

                image.Mutate(x => x.Resize(resizedWidth, resizedHeight));

                await image.SaveAsPngAsync(stream);

                await stream.FlushAsync();
                stream.Dispose();

                results.Add(new ImageMetadata
                {
                    Id = fullId,
                    Uri = new Uri(imageFile.Path),
                    Width = image.Width,
                    Height = image.Height
                });
            }

            _ongoingImageProcessingSemaphore.Release();

            return results;
        }
    }
}
