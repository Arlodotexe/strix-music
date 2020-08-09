using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.CoreInterfaces.Interfaces.CoreConfig
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
