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
    /// A class that handles turning a <see cref="ICoreUserProfile"/> into a <see cref="IUserProfile"/>.
    /// </summary>
    /// <remarks>
    /// User profiles are not actually merged (yet).
    /// </remarks>
    public class CoreUserProfileProxy : IUserProfile
    {
        private readonly ICoreUserProfile _userProfile;
        private readonly MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage> _imageMap;

        /// <summary>
        /// Creates a new instance of <see cref="CoreUserProfileProxy"/>.
        /// </summary>
        /// <param name="userProfile">The user to wrap around.</param>
        public CoreUserProfileProxy(ICoreUserProfile userProfile)
        {
            _userProfile = userProfile ?? ThrowHelper.ThrowArgumentNullException<ICoreUserProfile>(nameof(userProfile));

            Sources = _userProfile.IntoList();
            SourceCores = _userProfile.SourceCore.IntoList();

            _imageMap = new MergedCollectionMap<IImageCollection, ICoreImageCollection, IImage, ICoreImage>(this);

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
        public event EventHandler<int>? ImagesCountChanged
        {
            add => _userProfile.ImagesCountChanged += value;
            remove => _userProfile.ImagesCountChanged -= value;
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
        public int TotalImageCount => _userProfile.TotalImageCount;

        /// <inheritdoc />
        public string Id => _userProfile.Id;

        /// <inheritdoc />
        public string DisplayName => _userProfile.DisplayName;

        /// <inheritdoc />
        public string? FullName => _userProfile.FullName;

        /// <inheritdoc />
        public SynchronizedObservableCollection<Uri>? Urls => _userProfile.Urls;

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
        public Task<bool> IsAddUrlAvailable(int index)
        {
            return _userProfile.IsAddUrlAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index)
        {
            return _userProfile.IsAddImageAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailable(int index)
        {
            return _userProfile.IsRemoveUrlAvailable(index);
        }

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index)
        {
            return _userProfile.IsRemoveImageAvailable(index);
        }

        /// <inheritdoc />
        public Task RemoveImageAsync(int index)
        {
            return _userProfile.RemoveImageAsync(index);
        }

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName)
        {
            return _userProfile.ChangeDisplayNameAsync(displayName);
        }

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate)
        {
            return _userProfile.ChangeBirthDateAsync(birthdate);
        }

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname)
        {
            return _userProfile.ChangeFullNameAsync(fullname);
        }

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region)
        {
            return _userProfile.ChangeRegionAsync(region);
        }

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email)
        {
            return _userProfile.ChangeEmailAsync(email);
        }

        /// <inheritdoc />
        public IReadOnlyList<ICoreImageCollection> Sources { get; }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        public IReadOnlyList<ICore> SourceCores { get; }

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
            // User profiles are never merged.
            return false;
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync() => _userProfile.DisposeAsync();
    }
}