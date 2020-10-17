using System;
using StrixMusic.Sdk.Core.Data;

namespace StrixMusic.Sdk.Core.ViewModels
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUser"/>
    /// </summary>
    public class UserViewModel : UserProfileViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserViewModel"/> class.
        /// </summary>
        /// <param name="user">The <see cref="IUser"/> to wrap.</param>
        public UserViewModel(IUser user)
            : base(user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Library = new LibraryViewModel(user.Library);
        }

        /// <inheritdoc cref="ILibrary"/>
        public LibraryViewModel Library { get; }
    }
}
