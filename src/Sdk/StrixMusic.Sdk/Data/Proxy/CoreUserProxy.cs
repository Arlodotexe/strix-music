using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Toolkit.Diagnostics;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    /// <summary>
    /// A class that handles turning a <see cref="ICoreUser"/> into a <see cref="IUser"/>.
    /// </summary>
    /// <remarks>
    /// Users are not actually merged.
    /// </remarks>
    public class CoreUserProxy : IUser
    {
        private readonly ICoreUser _user;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;
        private readonly IReadOnlyList<ICoreImageCollection> _sources;

        /// <summary>
        /// Creates a new instance of <see cref="CoreUserProxy"/>.
        /// </summary>
        /// <param name="user">The user to wrap around.</param>
        public CoreUserProxy(ICoreUser user)
        {
            _user = user ?? ThrowHelper.ThrowArgumentNullException<ICoreUser>(nameof(user));
            
            SourceCore = _user.SourceCore;

            Library = new MergedLibrary(_user.Library.IntoList());

            // For image collection.
            _sources = _user.IntoList();
            SourceCores = _user.SourceCore.IntoList();
            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);

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
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _user.ImagesCountChanged += value;
            remove => _user.ImagesCountChanged -= value;
        }

        /// <inheritdoc />
        public event CollectionChangedEventHandler<IImage>? ImagesChanged;

        private void AttachEvents()
        {
            _imageMap.ItemsChanged += ImageMap_ItemsChanged;
        }

        private void DetachEvents()
        {
            _imageMap.ItemsChanged -= ImageMap_ItemsChanged;
        }

        private void ImageMap_ItemsChanged(object sender, IReadOnlyList<CollectionChangedItem<IImage>> addedItems, IReadOnlyList<CollectionChangedItem<IImage>> removedItems)
        {
            ImagesChanged?.Invoke(this, addedItems, removedItems);
        }

        /// <inheritdoc />
        public int TotalImageCount => _user.TotalImageCount;

        /// <inheritdoc />
        public string Id => _user.Id;

        /// <inheritdoc />
        public string DisplayName => _user.DisplayName;

        /// <inheritdoc />
        public string? FullName => _user.FullName;

        /// <inheritdoc />
        public SynchronizedObservableCollection<Uri>? Urls => _user.Urls;

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
        public Task<bool> IsAddUrlAvailable(int index)
        {
            return _user.IsAddUrlAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index)
        {
            return _user.IsAddImageAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailable(int index)
        {
            return _user.IsRemoveUrlAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return _user.IsRemoveImageAvailable(index);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            return _user.RemoveImageAsync(index);
        }

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName)
        {
            return _user.ChangeDisplayNameAsync(displayName);
        }

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate)
        {
            return _user.ChangeBirthDateAsync(birthdate);
        }

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname)
        {
            return _user.ChangeFullNameAsync(fullname);
        }

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region)
        {
            return _user.ChangeRegionAsync(region);
        }

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email)
        {
            return _user.ChangeEmailAsync(email);
        }

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        public IReadOnlyList<ICore> SourceCores { get; }

        /// <inheritdoc />
        public ILibrary Library { get; }

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset)
        {
            return _imageMap.GetItems(limit, offset);
        }

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index)
        {
            return _imageMap.InsertItem(image, index);
        }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other)
        {
            // Users are never merged.
            return false;
        }
    }
}