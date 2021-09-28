using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.Uno.Models;
using StrixMusic.Sdk.Uno.Services.ShellManagement;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace StrixMusic.Shared.ViewModels
{
    /// <summary>
    /// A view model containing metadata about a shell that the user can switch to.
    /// </summary>
    public class ShellInfoViewModel : ObservableObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShellViewModel"/> class.
        /// </summary>
        /// <param name="assemblyInfo">The shell assembly info to wrap around.</param>
        public ShellInfoViewModel(ShellMetadata metadata)
        {
            Metadata = metadata;

            IsFullyResponsive = metadata.MaxWindowSize.Height == double.PositiveInfinity &&
                                metadata.MaxWindowSize.Width == double.PositiveInfinity &&
                                metadata.MinWindowSize.Height == 0 &&
                                metadata.MinWindowSize.Width == 0;

            ShellPreviews = new ObservableCollection<Uri>();

            _ = SetupImages();
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

        private async Task SetupImages()
        {
            var foundFiles = new List<StorageFile>();

            // Brute find each image. We can't enumerate bundled content. A little hacky but it does the job.
            var index = 0;
            while (true)
            {
                try
                {
                    var fileName = $"{++index}.png";

                    var file = await StorageFile.GetFileFromApplicationUriAsync(new Uri($"ms-appx:///Assets/ShellPreviews/{Metadata.Id}/{fileName}"));

                    foundFiles.Add(file);
                }
                catch
                {
                    break;
                }
            }

            foreach (var file in foundFiles)
            {
                ShellPreviews.Add(new Uri($"ms-appx:///Assets/ShellPreviews/{Metadata.Id}/{file.Name}"));
            }
        }
    }
}
