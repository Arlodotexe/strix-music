using System;
using System.Collections.ObjectModel;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
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
            _userProfile = userProfile;
            Url = new ObservableCollection<Uri>(userProfile.Url);
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
        public ObservableCollection<IImage> Images { get; set; }

        /// <inheritdoc cref="IUserProfile.Url"/>
        public ObservableCollection<Uri>? Url { get; set; }

        /// <inheritdoc cref="IUserProfile.Region"/>
        public string? Region => _userProfile.Region;

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
        public event EventHandler<CollectionChangedEventArgs<DateTime>> FullNameChanged
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

        /// <inheritdoc cref="IUserProfile.UrlChanged"/>
        public event EventHandler<CollectionChangedEventArgs<DateTime>> UrlChanged
        {
            add
            {
                _userProfile.UrlChanged += value;
            }

            remove
            {
                _userProfile.UrlChanged -= value;
            }
        }

        /// <inheritdoc cref="IUserProfile.RegionChanged"/>
        public event EventHandler<CollectionChangedEventArgs<DateTime>> RegionChanged
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
        public event EventHandler<CollectionChangedEventArgs<DateTime>> EmailChanged
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
