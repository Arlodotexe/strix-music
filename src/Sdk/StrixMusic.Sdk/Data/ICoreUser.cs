namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Contains information about a user and their library.
    /// </summary>
    public interface ICoreUser : IUserProfile
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ILibrary Library { get; }
    }
}
