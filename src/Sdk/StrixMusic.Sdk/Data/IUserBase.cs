namespace StrixMusic.Sdk.Core.Data
{
    /// <summary>
    /// Contains information about a user and their library.
    /// </summary>
    public interface IUserBase : IUserProfileBase
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ILibraryBase Library { get; }
    }
}