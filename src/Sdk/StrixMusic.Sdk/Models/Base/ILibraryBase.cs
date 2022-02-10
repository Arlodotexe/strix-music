using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A group of collections that represent a music library.
    /// </summary>
    public interface ILibraryBase : IPlayableCollectionGroupBase, IAsyncDisposable
    {
    }
}
