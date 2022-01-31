using Microsoft.Toolkit.Diagnostics;
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
        /// Initializes a new instance of the <see cref="UserViewModel"/> class.
        /// </summary>
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
