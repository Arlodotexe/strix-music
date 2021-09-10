using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Cores.OneDrive.Models
{
    /// <summary>
    /// A LocalFileCore implementation of <see cref="ICoreSearchResults"/>.
    /// </summary>
    public class OneDriveCoreSearchResults : LocalFilesCoreSearchResults
    {

        ///<inheritdoc/>
        public OneDriveCoreSearchResults(ICore sourceCore, string query)
            : base(sourceCore, query)
        {
        }
    }
}
