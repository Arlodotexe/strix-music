// Copyright (c) Arlo Godfrey. All Rights Reserved.
// Licensed under the GNU Lesser General Public License, Version 3.0 with additional terms.
// See the LICENSE, LICENSE.LESSER and LICENSE.ADDITIONAL files in the project root for more information.

using CommunityToolkit.Diagnostics;
using StrixMusic.Sdk.Models;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUser"/>
    /// </summary>
    public class UserViewModel : UserProfileViewModel, ISdkViewModel
    {
        /// <summary>
        /// A ViewModel for <see cref="IUser"/>.
        /// </summary>
        /// <param name="root">The <see cref="MainViewModel"/> that this or the object that created this originated from.</param>
        /// <param name="user">The <see cref="IUser"/> to wrap.</param>
        public UserViewModel(MainViewModel root, IUser user)
            : base(root, user)
        {
            Guard.IsNotNull(user, nameof(user));

            Library = new LibraryViewModel(root, user.Library);
        }

        /// <inheritdoc cref="ILibraryBase"/>
        public LibraryViewModel Library { get; }
    }
}
