using System;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Observables
{
    /// <summary>
    /// Contains bindable information about an <see cref="IUser"/>
    /// </summary>
    public class ObservableUser : ObservableUserProfile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ObservableUser"/> class.
        /// </summary>
        /// <param name="user">The <see cref="IUser"/> to wrap.</param>
        public ObservableUser(IUser user)
            : base(user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            Library = new ObservableLibrary(user.Library);
        }

        /// <inheritdoc cref="ILibrary"/>
        public ObservableLibrary Library { get; }
    }
}
