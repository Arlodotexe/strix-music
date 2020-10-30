using System;
using System.Globalization;
using System.Threading.Tasks;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Core.MusicBrainz.Models
{
    /// <inheritdoc />
    public class MusicBrainzCoreUser : ICoreUser
    {
        /// <summary>
        /// Creates a new instance of a <see cref="MusicBrainzCoreUser"/>.
        /// </summary>
        /// <param name="sourceCore">The core instance that created this.</param>
        public MusicBrainzCoreUser(ICore sourceCore)
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
        public SynchronizedObservableCollection<IImage> Images { get; } = new SynchronizedObservableCollection<IImage>();

        /// <inheritdoc />
        public DateTime? Birthdate => null;

        /// <inheritdoc />
        public SynchronizedObservableCollection<Uri>? Urls { get; } = new SynchronizedObservableCollection<Uri>();

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
        public Task<bool> IsRemoveUrlSupported(int index)
        {
            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index)
        {
            return Task.FromResult(false);
        }

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
    }
}