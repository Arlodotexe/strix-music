using StrixMusic.CoreInterfaces.Interfaces;

namespace StrixMusic.ViewModels.Bindables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUser"/>
    /// </summary>
    public class BindableUser : BindableUserProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BindableUser"/> class.
        /// </summary>
        /// <param name="userProfile">The <see cref="IUser"/> to wrap.</param>
        public BindableUser(IUser user)
            : base(user)
        {
            Library = new BindableLibrary(user.Library);
        }

        /// <inheritdoc cref="ILibrary"/>
        public BindableLibrary Library { get; }
    }
}
