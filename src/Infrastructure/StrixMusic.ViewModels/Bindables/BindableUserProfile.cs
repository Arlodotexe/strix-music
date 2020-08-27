using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
using StrixMusic.CoreInterfaces;
using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUserProfile"/>
    /// </summary>
    public class BindableUserProfile : ObservableObject
    {
        private IUserProfile _userProfile;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        /// <param name="userProfile"></param>
        public BindableUserProfile(IUserProfile userProfile)
        {
            _userProfile = userProfile;
            Url = new ObservableCollection<Uri>(userProfile.Url);
            Images = new ObservableCollection<IImage>(userProfile.Images);
        }

        /// <summary>
        /// <inheritdoc cref="userProfile.SourceCore"/>
        /// </summary>
        public ICore SourceCore => _userProfile.SourceCore;

        /// <summary>
        /// <inheritdoc cref="userProfile.Id"/>
        /// </summary>
        public string Id => _userProfile.Id;

        /// <summary>
        /// <inheritdoc cref="userProfile.DisplayName"/>
        /// </summary>
        public string DisplayName => _userProfile.DisplayName;

        /// <summary>
        /// <inheritdoc cref="userProfile.FullName"/>
        /// </summary>
        public string? FullName => _userProfile.FullName;

        /// <summary>
        /// <inheritdoc cref="userProfile.Email"/>
        /// </summary>
        public string? Email => _userProfile.Email;

        /// <summary>
        /// <inheritdoc cref="userProfile.Birthdate"/>
        /// </summary>
        public DateTime? Birthdate => _userProfile.Birthdate;

        /// <summary>
        /// <inheritdoc cref="userProfile.Images"/>
        /// </summary>
        public ObservableCollection<IImage> Images { get; set; }

        /// <summary>
        /// <inheritdoc cref="userProfile.Url"/>
        /// </summary>
        public ObservableCollection<Uri>? Url { get; set; }

        /// <summary>
        /// <inheritdoc cref="userProfile.Region"/>
        /// </summary>
        public string? Region => _userProfile.Region;

        /// <summary>
        /// <inheritdoc cref="userProfile.DisplayNameChanged"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="userProfile.ImagesChanged"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="userProfile.BirthDateChanged"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="userProfile.FullNameChanged"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="userProfile.UrlChanged"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="userProfile.RegionChanged"/>
        /// </summary>
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

        /// <summary>
        /// <inheritdoc cref="userProfile.EmailChanged"/>
        /// </summary>
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
