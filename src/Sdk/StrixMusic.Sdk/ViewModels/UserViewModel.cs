using System;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="ICoreUser"/>
    /// </summary>
    public class UserViewModel : UserProfileViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewModel"/> class.
        /// </summary>
        /// <param name="user">The <see cref="ICoreUser"/> to wrap.</param>
        public UserViewModel(ICoreUser user)
            : base(user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Library = new LibraryViewModel(user.Library);
        }

        /// <inheritdoc cref="ILibraryBase"/>
        public LibraryViewModel Library { get; }
    }
}
