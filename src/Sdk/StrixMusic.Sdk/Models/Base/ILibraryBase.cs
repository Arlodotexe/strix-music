using System;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// Contains a music library.
    /// </summary>
    public interface ILibraryBase : IPlayableCollectionGroupBase, IAsyncDisposable
    {
    }
}
