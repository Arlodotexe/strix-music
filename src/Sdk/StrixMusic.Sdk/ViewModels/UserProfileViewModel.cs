using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using OwlCore.Collections;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ICoreUserProfile"/>
    /// </summary>
    public class UserProfileViewModel : ObservableObject, ICoreUserProfile
    {
        private readonly ICoreUserProfile _userProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserProfileViewModel"/> class.
        /// </summary>
        /// <param name="userProfile">The base <see cref="ICoreUserProfile"/></param>
        public UserProfileViewModel(ICoreUserProfile userProfile)
        {
            _userProfile = userProfile ?? throw new ArgumentNullException(nameof(userProfile));

            SourceCore = MainViewModel.GetLoadedCore(_userProfile.SourceCore);
            Urls = new SynchronizedObservableCollection<Uri>(userProfile.Urls);
            Images = new SynchronizedObservableCollection<ICoreImage>(userProfile.Images);
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
        public ICore SourceCore { get; }

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
        public SynchronizedObservableCollection<ICoreImage> Images { get; }

        /// <inheritdoc />
        public SynchronizedObservableCollection<Uri>? Urls { get; }

        /// <inheritdoc />
        public CultureInfo Region => _userProfile.Region;

        /// <inheritdoc />
        public bool IsChangeDisplayNameSupported => _userProfile.IsChangeDisplayNameSupported;

        /// <inheritdoc />
        public bool IsChangeBirthDateAsyncSupported => _userProfile.IsChangeBirthDateAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeFullNameAsyncAsyncSupported => _userProfile.IsChangeFullNameAsyncAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeRegionAsyncSupported => _userProfile.IsChangeRegionAsyncSupported;

        /// <inheritdoc />
        public bool IsChangeEmailAsyncSupported => _userProfile.IsChangeEmailAsyncSupported;

        /// <inheritdoc />
        public Task<bool> IsAddUrlSupported(int index) => _userProfile.IsAddUrlSupported(index);

        /// <inheritdoc />
        public Task<bool> IsAddImageSupported(int index) => _userProfile.IsAddImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveImageSupported(int index) => _userProfile.IsRemoveImageSupported(index);

        /// <inheritdoc />
        public Task<bool> IsRemoveUrlSupported(int index) => _userProfile.IsRemoveUrlSupported(index);

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
    }
}
