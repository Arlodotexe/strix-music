namespace StrixMusic.Sdk.Core.Data
{
    /// <inheritdoc cref="IUserBase"/>
    /// <remarks>This interface should be implemented by a core.</remarks>
    public interface ICoreUser : IUserBase, ICoreUserProfile, ICoreMember
    {
    }
}
