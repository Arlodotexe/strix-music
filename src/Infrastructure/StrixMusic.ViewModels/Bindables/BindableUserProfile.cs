using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Microsoft.Toolkit.Mvvm.ComponentModel;
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
        }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ICore SourceCore { get => _userProfile.SourceCore; set => _userProfile.SourceCore = value; }

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string Id => _userProfile.Id;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string DisplayName => _userProfile.DisplayName;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? FullName => _userProfile.FullName;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? Email => _userProfile.Email;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ObservableCollection<IImage> Images => (ObservableCollection<IImage>)_userProfile.Images;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public DateTime? Birthdate => _userProfile.Birthdate;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public ObservableCollection<Uri>? Url => (ObservableCollection<Uri>?)_userProfile.Url;

        /// <summary>
        /// <inheritdoc/>
        /// </summary>
        public string? Region { get => _userProfile.Region; set => _userProfile.Region = value; }
    }
}
