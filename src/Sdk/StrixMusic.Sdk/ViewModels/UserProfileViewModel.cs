using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using Microsoft.Toolkit.Mvvm.Input;
using OwlCore;
using OwlCore.Collections;
using OwlCore.Events;
using OwlCore.Extensions;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Core;
using StrixMusic.Sdk.Data.Merged;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUserProfile"/>
    /// </summary>
    public class UserProfileViewModel : ObservableObject, IUserProfile, IImageCollectionViewModel
    {
        private readonly IUserProfile _userProfile;
        private readonly IReadOnlyList<ICoreUserProfile> _sources;
        private readonly IReadOnlyList<ICore> _sourceCores;

        private List<Uri> _urls = new List<Uri>();

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileViewModel"/> class.
        /// </summary>
        /// <param name="userProfile">The base <see cref="IUserProfile"/></param>
        public UserProfileViewModel(IUserProfile userProfile)
        {
            // NOTES FOR LATER -- this class is WIP.
            // Finish refactoring user profile and user to use new IUrlCollection
            // !!! Cannot add IUrlCollection to (example) both an IAlbumCollection and ITrackCollection, you can't differentiate on an artist. (maybe you don't need to? evaluate me.)
            _userProfile = userProfile ?? throw new ArgumentNullException(nameof(userProfile));

            var userProfileImpl = userProfile.Cast<CoreUserProfileProxy>();

            _sourceCores = userProfileImpl.SourceCores.Select(MainViewModel.GetLoadedCore).ToList();
            _sources = userProfileImpl.Sources;

            PopulateMoreImagesCommand = new AsyncRelayCommand<int>(PopulateMoreImagesAsync);

            using (Threading.PrimaryContext)
            {
                if (userProfile.Urls != null)
                    _urls.AddRange(userProfile.Urls);

                Images = new ObservableCollection<IImage>();
            }
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
        public event CollectionChangedEventHandler<Uri>? UrlsChanged
        {
            add => _userProfile.UrlsChanged += value;
            remove => _userProfile.UrlsChanged -= value;
        }

        /// <inheritdoc cref="IMerged{T}.SourceCores" />
        IReadOnlyList<ICore> IMerged<ICoreImageCollection>.SourceCores => _sourceCores;

        /// <inheritdoc />
        IReadOnlyList<ICoreImageCollection> IMerged<ICoreImageCollection>.Sources => _sources;

        /// <inheritdoc />
        public string Id => _userProfile.Id;

        /// <inheritdoc />
        public int TotalImageCount => _userProfile.TotalImageCount;

        /// <inheritdoc />
        public string DisplayName => _userProfile.DisplayName;

        /// <inheritdoc />
        public string? FullName => _userProfile.FullName;

        /// <inheritdoc />
        public string? Email => _userProfile.Email;

        /// <inheritdoc />
        public DateTime? Birthdate => _userProfile.Birthdate;

        /// <inheritdoc />
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc />
        public IReadOnlyList<Uri>? Urls => _urls;

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
        public Task<bool> IsAddUrlAvailableAsync(int index) => _userProfile.IsAddUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageAvailable(int index) => _userProfile.IsAddImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageAvailable(int index) => _userProfile.IsRemoveImageAvailable(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlAvailableAsync(int index) => _userProfile.IsRemoveUrlAvailableAsync(index);

        /// <inheritdoc />
        public Task ChangeDisplayNameAsync(string displayName) => _userProfile.ChangeDisplayNameAsync(displayName);

        /// <inheritdoc />
        public Task ChangeBirthDateAsync(DateTime birthdate) => _userProfile.ChangeBirthDateAsync(birthdate);

        /// <inheritdoc />
        public Task ChangeFullNameAsync(string fullname) => _userProfile.ChangeFullNameAsync(fullname);

        /// <inheritdoc />
        public Task ChangeRegionAsync(CultureInfo region) => _userProfile.ChangeRegionAsync(region);

        /// <inheritdoc />
        public Task ChangeEmailAsync(string? email) => _userProfile.ChangeEmailAsync(email);

        /// <inheritdoc />
        public Task<IReadOnlyList<IImage>> GetImagesAsync(int limit, int offset) => _userProfile.GetImagesAsync(limit, offset);

        /// <inheritdoc />
        public Task RemoveImageAsync(int index) => _userProfile.RemoveImageAsync(index);

        /// <inheritdoc />
        public Task AddImageAsync(IImage image, int index) => _userProfile.AddImageAsync(image, index);

        /// <inheritdoc />
        public Task AddUrlAsync(Uri url, int index) => _userProfile.AddUrlAsync(url, index);

        /// <inheritdoc />
        public Task RemoveUrlAsync(int index) => _userProfile.RemoveUrlAsync(index);

        /// <inheritdoc />
        public async Task PopulateMoreImagesAsync(int limit)
        {
            var items = await GetImagesAsync(limit, Images.Count);

            _ = Threading.OnPrimaryThread(() =>
            {
                foreach (var item in items)
                {
                    Images.Add(item);
                }
            });
        }

        /// <inheritdoc />
        public IAsyncRelayCommand<int> PopulateMoreImagesCommand { get; }

        /// <inheritdoc />
        public bool Equals(ICoreImageCollection other)
        {
            return _userProfile.Equals(other);
        }

        /// <inheritdoc />
        public ValueTask DisposeAsync()
        {
            return _userProfile.DisposeAsync();
        }
    }
}
