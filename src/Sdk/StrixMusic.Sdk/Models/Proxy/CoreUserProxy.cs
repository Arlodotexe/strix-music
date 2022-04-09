// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Models.Core;
using StrixMusic.Sdk.Services;

namespace StrixMusic.Sdk.Models.Merged
{
    /// <summary>
    /// A class that handles turning a <see cref="ICoreUser"/> into a <see cref="IUser"/>.
    /// </summary>
    /// <remarks>
    /// Users are not actually merged.
    /// </remarks>
    public sealed class CoreUserProxy : IUser
    {
        private readonly ICoreUser _user;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl> _urlMap;
        private readonly IReadOnlyList<ICoreUser> _sources;

        /// <summary>
        /// Creates a new instance of <see cref="CoreUserProxy"/>.
        /// </summary>
        public CoreUserProxy(ICoreUser user)
        {
            _user = user ?? ThrowHelper.ThrowArgumentNullException<ICoreUser>(nameof(user));
            
            SourceCore = _user.SourceCore;

            TotalImageCount = user.TotalImageCount;
            TotalUrlCount = user.TotalUrlCount;

            Library = new MergedLibrary(_user.Library.IntoList(), new MergedCollectionConfig());

            SourceCores = _user.SourceCore.IntoList();
            _sources = _user.IntoList();

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this, new MergedCollectionConfig());
            _urlMap = new MergedCollectionMap<IUrlCollection, ICoreUrlCollection, IUrl, ICoreUrl>(this, new MergedCollectionConfig());

            AttachEvents();
        }

        /// <inheritdoc />
        public ICore? SourceCore { get; set; }

        /// <inheritdoc />
        public ICoreUser? Source { get; set; }

        /// <inheritdoc />
        public event EventHandler<string>? DisplayNameChanged
        {
            add => _user.DisplayNameChanged += value;
            remove => _user.DisplayNameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<DateTime>? BirthDateChanged
        {
            add => _user.BirthDateChanged += value;
            remove => _user.BirthDateChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string>? FullNameChanged
        {
            add => _user.FullNameChanged += value;
            remove => _user.FullNameChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<CultureInfo>? RegionChanged
        {
            add => _user.RegionChanged += value;
            remove => _user.RegionChanged -= value;
        }

        /// <inheritdoc />
        public event EventHandler<string?>? EmailChanged
        {
            add => _user.EmailChanged += value;
            remove => _user.EmailChanged -= value;
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
            => ImagesChanged?.Invoke(this, addedItems, removedItems);

        private void UrlMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IUrl>> addedItems, IReadOnlyList<CollectionChangedItem<IUrl>> removedItems)
            => UrlsChanged?.Invoke(this, addedItems, removedItems);

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
        public string Id => _user.Id;

        /// <inheritdoc />
        public string DisplayName => _user.DisplayName;

        /// <inheritdoc />
        public string? FullName => _user.FullName;

        /// <inheritdoc />
        public string? Email => _user.Email;

        /// <inheritdoc />
        public DateTime? Birthdate => _user.Birthdate;

        /// <inheritdoc />
        public CultureInfo Region => _user.Region;

        /// <inheritdoc />
        public bool IsChangeDisplayNameAvailable => _user.IsChangeDisplayNameAvailable;

        /// <inheritdoc />
        public bool IsChangeBirthDateAsyncAvailable => _user.IsChangeBirthDateAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeFullNameAsyncAsyncAvailable => _user.IsChangeFullNameAsyncAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeRegionAsyncAvailable => _user.IsChangeRegionAsyncAvailable;

        /// <inheritdoc />
        public bool IsChangeEmailAsyncAvailable => _user.IsChangeEmailAsyncAvailable;

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _user.IsAddImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailableAsync(int index, CancellationToken cancellationToken = default) => _user.IsRemoveImageAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsAddUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _user.IsAddUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index, CancellationToken cancellationToken = default) => _user.IsRemoveUrlAvailableAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName, CancellationToken cancellationToken = default) => _user.ChangeDisplayNameAsync(displayName, cancellationToken);

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate, CancellationToken cancellationToken = default) => _user.ChangeBirthDateAsync(birthdate, cancellationToken);

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname, CancellationToken cancellationToken = default) => _user.ChangeFullNameAsync(fullname, cancellationToken);

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region, CancellationToken cancellationToken = default) => _user.ChangeRegionAsync(region, cancellationToken);

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email, CancellationToken cancellationToken = default) => _user.ChangeEmailAsync(email, cancellationToken);

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        IReadOnlyList<ICoreUrlCollection> IMerged<ICoreUrlCollection>.Sources => _sources;

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        public ILibrary Library { get; }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset, CancellationToken cancellationToken = default) => _imageMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task<IReadOnlyList<IUrl>> GetUrlsAsync(int limit, int offset, CancellationToken cancellationToken = default) => _urlMap.GetItemsAsync(limit, offset, cancellationToken);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index, CancellationToken cancellationToken = default) => _imageMap.InsertItemAsync(image, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index, CancellationToken cancellationToken = default) => _user.RemoveImageAsync(index, cancellationToken);

        /// <inheritdoc />
        public Task AddUrlAsync(IUrl url, int index, CancellationToken cancellationToken = default) => _urlMap.InsertItemAsync(url, index, cancellationToken);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index, CancellationToken cancellationToken = default) => _urlMap.RemoveAtAsync(index, cancellationToken);

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other) =>
            // Users are never merged.
            false;

        /// <inheritdoc />
        public bool Equals(ICoreUrlCollection other) =>
            // User profiles are never merged.
            false;

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            DetachEvents();
            return _user.DisposeAsync();
        }
    }
}
