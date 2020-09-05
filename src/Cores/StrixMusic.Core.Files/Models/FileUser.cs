using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.Core.Files.Models
{
    /// <summary>
    /// Represents a the user that is accessing files.
    /// </summary>
    public class FileUser : IUser
    {
        /// <summary>
        /// Constructs a new <see cref="FileUser"/>.
        /// </summary>
        /// <param name="sourceCore"></param>
        public FileUser(ICore sourceCore)
        {
            SourceCore = sourceCore;
        }

        /// <inheritdoc/>
        public IReadOnlyList<IDevice> Devices => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Id => nameof(FileUser);

        /// <inheritdoc/>
        public string DisplayName => throw new NotImplementedException();

        /// <inheritdoc/>
        public string FullName => throw new NotImplementedException();

        /// <inheritdoc/>
        public string Email => throw new NotImplementedException();

        /// <inheritdoc/>
        public IReadOnlyList<IImage> Images => throw new NotImplementedException();

        /// <inheritdoc/>
        public DateTime? Birthdate => null;

        /// <inheritdoc/>
        public IReadOnlyList<Uri> Urls => throw new NotImplementedException();

        /// <inheritdoc/>
        public CultureInfo Region { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <inheritdoc/>
        public ICore SourceCore { get; set; }

        /// <inheritdoc/>
        public ILibrary Library => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeDisplayNameSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeImagesAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeBirthDateAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeFullNameAsyncAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeUrlsAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeRegionAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public bool IsChangeEmailAsyncSupported => throw new NotImplementedException();

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<string>>? DisplayNameChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<DateTime>>? BirthDateChanged;

        /// <inheritdoc/>
        public event EventHandler<string>? FullNameChanged;

        /// <inheritdoc/>
        public event EventHandler<CollectionChangedEventArgs<Uri>>? UrlsChanged;

        /// <inheritdoc/>
        public event EventHandler<CultureInfo>? RegionChanged;

        /// <inheritdoc/>
        public event EventHandler<string?>? EmailChanged;

        /// <inheritdoc/>
        public Task ChangeBirthDateAsync(DateTime birthdate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeDisplayNameAsync(string displayName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeEmailAsync(string? email)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeFullNameAsync(string fullname)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeRegionAsync(CultureInfo region)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public Task ChangeUrlsAsync(IReadOnlyList<Uri> urls)
        {
            throw new NotImplementedException();
        }
    }
}
