using System;
using System.Collections.Generic;

namespace StrixMusic.CoreInterfaces.Interfaces.CoreConfig
{
    /// <summary>
    /// Provides various methods of configuring a core.
    /// </summary>
    public interface ICoreConfig
    {
        /// <summary>
        /// <inheritdoc cref="IFileConfig"/>
        /// </summary>
        IFileConfig FileConfig { get; set; }
    }

    /// <summary>
    /// Provides methods to configure files access.
    /// </summary>
    public interface IFileConfig
    {
        /// <summary>
        /// Sets a list of file paths that the user has allowed access.
        /// </summary>
        void SetFileAccessList(IReadOnlyList<string> filePaths);
    }
}
