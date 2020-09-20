namespace StrixMusic.Sdk.Interfaces
{
    /// <summary>
    /// Contains information about a user and their library.
    /// </summary>
    public interface IUser : IUserProfile
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ILibrary Library { get; }
    }
}
