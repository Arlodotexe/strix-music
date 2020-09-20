using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Threading.Tasks;
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
        }

        /// <inheritdoc />
        public ICore SourceCore { get; }

        /// <inheritdoc />
        public ILibrary Library => SourceCore.Library;

        /// <inheritdoc />
        public string Id => SourceCore.InstanceId;

        /// <inheritdoc />
        public string DisplayName => "Anonymous";

        /// <inheritdoc />
        public string? FullName => null;

        /// <inheritdoc />
        public string? Email => null;

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; } = new ObservableCollection<IImage>();

        /// <inheritdoc />
        public DateTime? Birthdate => null;

        /// <inheritdoc />
        public ObservableCollection<Uri>? Urls { get; } = new ObservableCollection<Uri>();

        /// <inheritdoc />
        public CultureInfo Region => CultureInfo.CurrentUICulture;

        /// <inheritdoc />
        public bool IsChangeDisplayNameSupported => false;

        /// <inheritdoc />
        public bool IsChangeBirthDateAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeFullNameAsyncAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeRegionAsyncSupported => false;

        /// <inheritdoc />
        public bool IsChangeEmailAsyncSupported => false;

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemoveImageSupportedMap { get; } = new ObservableCollection<bool>();

        /// <inheritdoc />
        public ObservableCollection<bool> IsRemoveUrlSupportedMap { get; } = new ObservableCollection<bool>();

        /// <inheritdoc />
        public Task<bool> IsAddUrlSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public event EventHandler<string>? FullNameChanged;

        /// <inheritdoc />
        public event EventHandler<CultureInfo>? RegionChanged;

        /// <inheritdoc />
        public event EventHandler<string?>? EmailChanged;

        /// <inheritdoc />
        public event EventHandler<string>? DisplayNameChanged;

        /// <inheritdoc />
        public event EventHandler<DateTime>? BirthDateChanged;

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region)
        {
            throw new NotSupportedException();
        }

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email)
        {
            throw new NotSupportedException();
        }
    }
}