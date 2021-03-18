using System;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Base
{
    /// <summary>
    /// A <see cref="ICore"/> is a common API surface that can be implemented to interface Strix with an arbitrary music service provider.
    /// </summary>
    public interface ICoreBase : IAsyncDisposable
    {
    }
}
