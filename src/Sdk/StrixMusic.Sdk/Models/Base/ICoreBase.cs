using System;
using StrixMusic.Sdk.Models.Core;

namespace StrixMusic.Sdk.Models.Base
{
    /// <summary>
    /// A <see cref="ICore"/> is a common API surface that can be implemented to interface Strix with an arbitrary music service provider.
    /// </summary>
    public interface ICoreBase : IAsyncDisposable
    {
    }
}
