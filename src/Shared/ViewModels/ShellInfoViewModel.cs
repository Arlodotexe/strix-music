using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.WinUI.Services.ShellManagement;
using Windows.Storage;
using OwlCore.ComponentModel;

namespace StrixMusic.ViewModels
{
    /// <summary>
    /// A view model containing metadata about a shell that the user can switch to.
    /// </summary>
    public class ShellInfoViewModel : ObservableObject, IAsyncInit
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellInfoViewModel"/> class.
        /// </summary>
        /// <param name="metadata">The shell metadata to wrap around.</param>
        public ShellInfoViewModel(ShellMetadata metadata)
        {
            Metadata = metadata;

            IsFullyResponsive = metadata.MaxWindowSize.Height == double.PositiveInfinity &&
                                metadata.MaxWindowSize.Width == double.PositiveInfinity &&
                                metadata.MinWindowSize.Height == 0 &&
                                metadata.MinWindowSize.Width == 0;

            ShellPreviews = new ObservableCollection<Uri>();
        }

        /// <inheritdoc cref="ShellMetadata"/>
        public ShellMetadata Metadata { get; }

        /// <summary>
        /// <see cref="Uri"/>s pointing to preview images for this shell. Ordered Alphanumerically based on file name. 
        /// </summary>
        public ObservableCollection<Uri> ShellPreviews { get; set; }

        /// <summary>
        /// If true, this shell is capable of supporting all screen sizes.
        /// </summary>
        public bool IsFullyResponsive { get; }

        /// <inheritdoc cref="ShellMetadata.DisplayName"/>
        public string DisplayName => Metadata.DisplayName;

        /// <inheritdoc cref="ShellMetadata.Description"/>
        public string Description => Metadata.Description;

        /// <inheritdoc/>
        public bool IsInitialized { get; private set; }

        /// <inheritdoc/>
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            return SetupImages(cancellationToken);
        }

        private async Task SetupImages(CancellationToken cancellationToken)
        {
            var foundFiles = new List<StorageFile>();

            // Brute find each image. We can't enumerate bundled content. A little hacky but it does the job.
            for (int index = 0; index < 100; index++)
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var fileName = $"{++index}.png";

                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/ShellPreviews/{Metadata.Id}/{fileName}"));

                    foundFiles.Add(file);
                }
                catch
                {
                    break;
                }
            }

            await OwlCore.Threading.OnPrimaryThread(() =>
            {
                foreach (var file in foundFiles)
                {
                    ShellPreviews.Add(new Uri($"ms-appx:///Assets/ShellPreviews/{Metadata.Id}/{file.Name}"));
                }
            });
        }
    }
}
