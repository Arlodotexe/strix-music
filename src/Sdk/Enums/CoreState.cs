using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Sdk.Interfaces;

namespace StrixMusic.Sdk.Enums
{
    /// <summary>
    /// The state of a <see cref="ICore"/>.
    /// </summary>
    public enum CoreState
    {
        Unloaded,
        Loading,
        Loaded,
        Faulted,
    }
}
