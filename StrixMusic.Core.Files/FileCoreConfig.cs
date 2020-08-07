using StrixMusic.CoreInterfaces.Interfaces.CoreConfig;
using System.Collections.Generic;

namespace StrixMusic.Core.Files
{
    /// <summary>
    /// Configuration for the <see cref="FileCore"/>
    /// </summary>
    public class FileCoreConfig : ICoreConfig
    {
        /// <inheritdoc/>
        public IFileConfig FileConfig { get; set; }
    }

    public class FileConfig : IFileConfig
    {
        
        public void SetFileAccessList(IReadOnlyList<string> filePaths)
        {
            
        }
    }
}
