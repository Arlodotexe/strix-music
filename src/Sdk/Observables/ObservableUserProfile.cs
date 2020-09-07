using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUserProfile"/>
    /// </summary>
    public class ObservableUserProfile : ObservableObject
    {
        private IUserProfile _userProfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableUserProfile"/> class.
        /// </summary>
        /// <param name="userProfile">The base <see cref="IUserProfile"/></param>
        public ObservableUserProfile(IUserProfile userProfile)
        {
            if (userProfile == null)
            {
                throw new ArgumentNullException(nameof(userProfile));
            }

            _userProfile = userProfile;

            Urls = new ObservableCollection<Uri>(userProfile.Urls);
            Images = new ObservableCollection<IImage>(userProfile.Images);
        }

        /// <inheritdoc cref="IUserProfile.SourceCore"/>
        public ICore SourceCore => _userProfile.SourceCore;

        /// <inheritdoc cref="IUserProfile.Id"/>
        public string Id => _userProfile.Id;

        /// <inheritdoc cref="IUserProfile.DisplayName"/>
        public string DisplayName => _userProfile.DisplayName;

        /// <inheritdoc cref="IUserProfile.FullName"/>
        public string? FullName => _userProfile.FullName;

        /// <inheritdoc cref="IUserProfile.Email"/>
        public string? Email => _userProfile.Email;

        /// <inheritdoc cref="IUserProfile.Birthdate"/>
        public DateTime? Birthdate => _userProfile.Birthdate;

        /// <inheritdoc cref="IUserProfile.Images"/>
        public ObservableCollection<IImage> Images { get; }

        /// <inheritdoc cref="IUserProfile.Urls"/>
        public ObservableCollection<Uri>? Urls { get; }

        /// <inheritdoc cref="IUserProfile.Region"/>
        public CultureInfo? Region => _userProfile.Region;

        /// <inheritdoc cref="IUserProfile.IsChangeDisplayNameSupported"/>
        public bool IsChangeDisplayNameSupported { get; }

        /// <inheritdoc cref="IUserProfile.IsChangeImagesAsyncSupported"/>
        public bool IsChangeImagesAsyncSupported { get; }

        /// <inheritdoc cref="IUserProfile.IsChangeBirthDateAsyncSupported"/>
        public bool IsChangeBirthDateAsyncSupported { get; }

        /// <inheritdoc cref="IUserProfile.IsChangeFullNameAsyncAsyncSupported"/>
        public bool IsChangeFullNameAsyncAsyncSupported { get; }

        /// <inheritdoc cref="IUserProfile.IsChangeUrlsAsyncSupported"/>
        public bool IsChangeUrlsAsyncSupported { get; }

        /// <inheritdoc cref="IUserProfile.IsChangeRegionAsyncSupported"/>
        public bool IsChangeRegionAsyncSupported { get; }

        /// <inheritdoc cref="IUserProfile.IsChangeEmailAsyncSupported"/>
        public bool IsChangeEmailAsyncSupported { get; }

        /// <inheritdoc cref="IUserProfile.DisplayNameChanged"/>
        public event EventHandler<CollectionChangedEventArgs<string>> DisplayNameChanged
        {
            add
            {
                _userProfile.DisplayNameChanged += value;
            }

            remove
            {
                _userProfile.DisplayNameChanged -= value;
            }
        }

        /// <inheritdoc cref="IUserProfile.ImagesChanged"/>
        public event EventHandler<CollectionChangedEventArgs<IImage>> ImagesChanged
        {
            add
            {
                _userProfile.ImagesChanged += value;
            }

            remove
            {
                _userProfile.ImagesChanged -= value;
            }
        }

        /// <inheritdoc cref="IUserProfile.BirthDateChanged"/>
        public event EventHandler<CollectionChangedEventArgs<DateTime>> BirthDateChanged
        {
            add
            {
                _userProfile.BirthDateChanged += value;
            }

            remove
            {
                _userProfile.BirthDateChanged -= value;
            }
        }

        /// <inheritdoc cref="IUserProfile.FullNameChanged"/>
        public event EventHandler<string> FullNameChanged
        {
            add
            {
                _userProfile.FullNameChanged += value;
            }

            remove
            {
                _userProfile.FullNameChanged -= value;
            }
        }

        /// <inheritdoc cref="IUserProfile.UrlsChanged"/>
        public event EventHandler<CollectionChangedEventArgs<Uri>> UrlChanged
        {
            add
            {
                _userProfile.UrlsChanged += value;
            }

            remove
            {
                _userProfile.UrlsChanged -= value;
            }
        }

        /// <inheritdoc cref="IUserProfile.RegionChanged"/>
        public event EventHandler<CultureInfo> RegionChanged
        {
            add
            {
                _userProfile.RegionChanged += value;
            }

            remove
            {
                _userProfile.RegionChanged -= value;
            }
        }

        /// <inheritdoc cref="IUserProfile.EmailChanged"/>
        public event EventHandler<string?> EmailChanged
        {
            add
            {
                _userProfile.EmailChanged += value;
            }

            remove
            {
                _userProfile.EmailChanged -= value;
            }
        }
    }
}
