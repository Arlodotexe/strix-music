using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using StrixMusic.Sdk.Events;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzUser : IUser
    {
        /// <summary>
        /// Creates a new instance of a <see cref="MusicBrainzUser"/>.
        /// </summary>
        /// <param name="sourceCore">The core instance that created this.</param>
        public MusicBrainzUser(ICore sourceCore)
        {
            SourceCore = sourceCore;
            Images = new List<IImage>();
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public string Id => SourceCore.InstanceId;

        /// <inheritdoc />
        public string DisplayName => "Anonymous";

        /// <inheritdoc />
        public string? FullName => null;

        /// <inheritdoc />
        public string? Email => null;

        /// <inheritdoc />
        public IReadOnlyList<IImage> Images { get; }

        /// <inheritdoc />
        public DateTime? Birthdate => null;

        /// <inheritdoc />
        public IReadOnlyList<Uri>? Urls => null;

        /// <inheritdoc />
        public CultureInfo Region => CultureInfo.CurrentUICulture;

        /// <inheritdoc />
        public bool IsChangeDisplayNameSupported => false;

        /// <inheritdoc />
        public bool IsChangeImagesAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeBirthDateAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeFullNameAsyncAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeUrlsAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeRegionAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeEmailAsyncSupported => false;

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ChangeImagesAsync(IReadOnlyList<IImage> images)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ChangeUrlsAsync(IReadOnlyList<Uri> urls)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<string>>? DisplayNameChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<IImage>>? ImagesChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<DateTime>>? BirthDateChanged;

        /// <inheritdoc />
        public event EventHandler<string>? FullNameChanged;

        /// <inheritdoc />
        public event EventHandler<CollectionChangedEventArgs<Uri>>? UrlsChanged;

        /// <inheritdoc />
        public event EventHandler<CultureInfo>? RegionChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? EmailChanged;

        /// <inheritdoc />
        public ILibrary Library => SourceCore.Library;
    }
}