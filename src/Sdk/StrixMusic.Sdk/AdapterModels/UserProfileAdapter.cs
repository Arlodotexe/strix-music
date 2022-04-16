// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using CommunityToolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.AppModels;
using StrixMusic.Sdk.CoreModels;

namespace StrixMusic.Sdk.AdapterModels
{
    /// <summary>
    /// A class that handles turning a <see cref="ICoreUserProfile"/> into a <see cref="IUserProfile"/>.
    /// </summary>
    /// <remarks>
    /// User profiles are not actually merged (yet).
    /// </remarks>
    public sealed class UserProfileAdapter : IUserProfile
    {
        private readonly ICoreUserProfile _userProfile;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlMap;
        private readonly IReadOnlyList<ICoreUserProfile> _sources;

        /// <summary>
        /// Creates a new instance of <see cref="UserProfileAdapter"/>.
        /// </summary>
        public UserProfileAdapter(ICoreUserProfile userProfile)
        {
            _userProfile = userProfile ?? ThrowHelper.ThrowArgumentNullException<ICoreUserProfile>(nameof(userProfile));

            _sources = _userProfile.IntoList();
            SourceCores = _userProfile.SourceCore.IntoList();

            TotalImageCount = _userProfile.TotalImageCount;
            TotalUrlCount = _userProfile.TotalUrlCount;

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, new MergedCollectionConfig());
            _urlMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, new MergedCollectionConfig());

            AttachEvents();
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
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IUrl>? UrlsChanged;

        /// <inheritdoc />
        public event EventHandler<int>? ImagesCountChanged;

        /// <inheritdoc />
        public event EventHandler<int>? UrlsCountChanged;

        private void AttachEvents()
        {
            _imageMap.ItemsChanged += ImageMap_ItemsChanged;
            _imageMap.ItemsCountChanged += ImageMap_ItemsCountChanged;

            _urlMap.ItemsChanged += UrlMap_ItemsChanged;
            _urlMap.ItemsCountChanged += UrlMap_ItemsCountChanged;
        }

        private void DetachEvents()
        {
            _imageMap.ItemsChanged -= ImageMap_ItemsChanged;
            _imageMap.ItemsCountChanged -= ImageMap_ItemsCountChanged;

            _urlMap.ItemsChanged -= UrlMap_ItemsChanged;
            _urlMap.ItemsCountChanged -= UrlMap_ItemsCountChanged;
        }

        private void ImageMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
        {
            UrlsChanged?.Invoke(this, addedItems, removedItems);
        }

        private void UrlMap_ItemsCountChanged(object sender, int e)
        {
            TotalUrlCount = e;
            UrlsCountChanged?.Invoke(this, TotalUrlCount);
        }

        private void ImageMap_ItemsCountChanged(object sender, int e)
        {
            TotalImageCount = e;
            ImagesCountChanged?.Invoke(this, TotalImageCount);
        }

        /// <inheritdoc />
        public int TotalImageCount { get; private set; }

        /// <inheritdoc/>
        public int TotalUrlCount { get; private set; }

        /// <inheritdoc />
        public string Id => _userProfile.Id;

        /// <inheritdoc />
        public string DisplayName => _userProfile.DisplayName;

        /// <inheritdoc />
        public string? FullName => _userProfile.FullName;

        /// <inheritdoc />
        public string? Email => _userProfile.Email;

        /// <inheritdoc />
        public DateTime? Birthdate => _userProfile.Birthdate;

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
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return _userProfile.IsAddImageAvailableAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return _userProfile.IsRemoveImageAvailableAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return _userProfile.IsAddUrlAvailableAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default)
        {
            return _userProfile.IsRemoveUrlAvailableAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName, CancellationToken cancellationToken = default)
        {
            return _userProfile.ChangeDisplayNameAsync(displayName, cancellationToken);
        }

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate, CancellationToken cancellationToken = default)
        {
            return _userProfile.ChangeBirthDateAsync(birthdate, cancellationToken);
        }

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname, CancellationToken cancellationToken = default)
        {
            return _userProfile.ChangeFullNameAsync(fullname, cancellationToken);
        }

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region, CancellationToken cancellationToken = default)
        {
            return _userProfile.ChangeRegionAsync(region, cancellationToken);
        }

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email, CancellationToken cancellationToken = default)
        {
            return _userProfile.ChangeEmailAsync(email, cancellationToken);
        }

        /// <summary>
        /// The sources that were combined to form the data in this instance.
        /// </summary>
        /// <remarks>
        /// Use this properly, but do not yet rely on it. Merging of users and user profiles are TODO.
        /// </remarks>
        public IReadOnlyList<ICoreUserProfile> Sources => _sources; 

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => _sources;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _imageMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default)
        {
            return _urlMap.GetItemsAsync(limit, offset, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default)
        {
            return _imageMap.InsertItemAsync(image, index, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default)
        {
            return _userProfile.RemoveImageAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default)
        {
            return _urlMap.InsertItemAsync(url, index, cancellationToken);
        }

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default)
        {
            return _urlMap.RemoveAtAsync(index, cancellationToken);
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other)
        {
            // User profiles are never merged.
            return false;
        }

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other)
        {
            // User profiles are never merged.
            return false;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _userProfile.DisposeAsync();
        }
    }
}
