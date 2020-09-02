using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUser"/>
    /// </summary>
    public class ObservableUser : ObservableUserProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableUser"/> class.
        /// </summary>
        /// <param name="userProfile">The <see cref="IUser"/> to wrap.</param>
        public ObservableUser(IUser user)
            : base(user)
        {
            Library = new ObservableLibrary(user.Library);
        }

        /// <inheritdoc cref="ILibrary"/>
        public ObservableLibrary Library { get; }
    }
}
