// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.AdapterModels;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;
using StrixMusic.Sdk.ViewModels.Helpers;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// A ViewModel for <see cref="IUserProfile"/>.
    /// </summary>
    public class UserProfileViewModel : ObservableObject, ISdkViewModel, IUserProfile, IImageCollectionViewModel, IUrlCollectionViewModel
    {
        private readonly IUserProfile _userProfile;
        private readonly IReadOnlyList<ICoreUserProfile> _sources;
        private readonly IReadOnlyList<ICore> _sourceCores;
        private readonly SynchronizationContext _syncContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileViewModel"/> class.
        /// </summary>
        /// <param name="userProfile">The base <see cref="IUserProfile"/></param>
        internal UserProfileViewModel(IUserProfile userProfile)
        {
            _syncContext = SynchronizationContext.Current;

            _userProfile = userProfile ?? throw new ArgumentNullException(nameof(userProfile));

            var userProfileImpl = userProfile.Cast<UserProfileAdapter>();

            _sourceCores = userProfileImpl.SourceCores.Select(x => new CoreViewModel(x)).ToList();
            _sources = userProfileImpl.Sources;

            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);
            PopulateMoreUrlsCommand = new AsyncRelayCommand<int>(PopulateMoreUrlsAsync);

            InitImageCollectionAsyncCommand = new AsyncRelayCommand(InitImageCollectionAsync);

            Images = new ObservableCollection<IImage>();
            Urls = new ObservableCollection<IUrl>();
        }

        /// <inheritdoc />
        public Task InitAsync(CancellationToken cancellationToken = default)
        {
            if (IsInitialized)
                return Task.CompletedTask;

            IsInitialized = true;

            return InitImageCollectionAsync(cancellationToken);
        }

        /// <inheritdoc />
        public event EventHandler<string>? DisplayNameChanged
        {
            add => _userProfile.DisplayNameChanged += value;
            remove => _userProfile.DisplayNameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime>? BirthDateChanged
        {
            add => _userProfile.BirthDateChanged += value;
            remove => _userProfile.BirthDateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? FullNameChanged
        {
            add => _userProfile.FullNameChanged += value;
            remove => _userProfile.FullNameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CultureInfo>? RegionChanged
        {
            add => _userProfile.RegionChanged += value;
            remove => _userProfile.RegionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? EmailChanged
        {
            add => _userProfile.EmailChanged += value;
            remove => _userProfile.EmailChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _userProfile.ImagesCountChanged += value;
            remove => _userProfile.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged
        {
            add => _userProfile.ImagesChanged += value;
            remove => _userProfile.ImagesChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged
        {
            add => _userProfile.UrlsCountChanged += value;
            remove => _userProfile.UrlsCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged
        {
            add => _userProfile.UrlsChanged += value;
            remove => _userProfile.UrlsChanged -= value;
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        IReadOnlyList<ICore> IMerged<ICoreImageCollection>.SourceCores => _sourceCores;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        IReadOnlyList<ICore> IMerged<ICoreUrlCollection>.SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => _sources;

        /// <inheritdoc />
        public string Id => _userProfile.Id;

        /// <inheritdoc />
        public int TotalImageCount => _userProfile.TotalImageCount;

        /// <inheritdoc />
        public int TotalUrlCount => _userProfile.TotalUrlCount;

        /// <inheritdoc />
        public string DisplayName => _userProfile.DisplayName;

        /// <inheritdoc />
        public string? FullName => _userProfile.FullName;

        /// <inheritdoc />
        public string? Email => _userProfile.Email;

        /// <inheritdoc />
        public DateTime? Birthdate => _userProfile.Birthdate;

        /// <inheritdoc />
        public bool IsInitialized { get; private set; }

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public ObservableCollection<IUrl> Urls { get; }

        /// <inheritdoc />
        public CultureInfo Region => _userProfile.Region;

        /// <inheritdoc />
        public bool IsChangeDisplayNameAvailable => _userProfile.IsChangeDisplayNameAvailable;

        /// <inheritdoc />
        public bool IsChangeBirthDateAsyncAvailable => _userProfile.IsChangeBirthDateAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeFullNameAsyncAsyncAvailable => _userProfile.IsChangeFullNameAsyncAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeRegionAsyncAvailable => _userProfile.IsChangeRegionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeEmailAsyncAvailable => _userProfile.IsChangeEmailAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _userProfile.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _userProfile.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _userProfile.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _userProfile.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName, CancellationToken cancellationToken = default) => _userProfile.ChangeDisplayNameAsync(displayName, cancellationToken);

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate, CancellationToken cancellationToken = default) => _userProfile.ChangeBirthDateAsync(birthdate, cancellationToken);

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname, CancellationToken cancellationToken = default) => _userProfile.ChangeFullNameAsync(fullname, cancellationToken);

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region, CancellationToken cancellationToken = default) => _userProfile.ChangeRegionAsync(region, cancellationToken);

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email, CancellationToken cancellationToken = default) => _userProfile.ChangeEmailAsync(email, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _userProfile.GetImagesAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _userProfile.GetUrlsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _userProfile.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _userProfile.AddImageAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _userProfile.AddUrlAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _userProfile.RemoveUrlAsync(index, cancellationToken);

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit, CancellationToken cancellationToken = default)
        {
            var items = await GetImagesAsync(limit, Images.Count, cancellationToken);

            _syncContext.Post(_ =>
            {
                foreach (var item in items)
                    Images.Add(item);
            }, null);
        }

        /// <inheritdoc />
        public async Task PopulateMoreUrlsAsync(int limit, CancellationToken cancellationToken = default)
        {
            var items = await GetUrlsAsync(limit, Urls.Count, cancellationToken);

            _syncContext.Post(_ =>
            {
                foreach (var item in items)
                    Urls.Add(item);
            }, null);
        }

        /// <inheritdoc />
        public Task InitImageCollectionAsync(CancellationToken cancellationToken = default) => CollectionInit.ImageCollection(this, cancellationToken);

        /// <inheritdoc />
        public IAsyncRelayCommand InitImageCollectionAsyncCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreUrlsCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) => _userProfile.Equals(other);

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) => _userProfile.Equals(other);

        /// <inheritdoc />
        public ValueTask DisposeAsync() => _userProfile.DisposeAsync();
    }
}
