namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IUserBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface IUser : IUserBase, IUserProfile, ISdkMember
    {
        /// <summary>
        /// This user's library.
        /// </summary>
        ILibrary Library { get; }
    }
}
