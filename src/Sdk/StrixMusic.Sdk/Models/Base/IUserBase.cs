using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains information about a user and their library.
    /// </summary>
    public interface IUserBase : IUserProfileBase, IAsyncDisposable
    {
    }
}