using StrixMusic.Core.LocalFiles;
using System;
using System.Collections.Generic;
using System.Text;

namespace StrixMusic.Core.OneDriveCore
{
    ///<inheritdoc/>
    public class OneDriveCore : LocalFilesCore
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneDriveCore"/> class.
        /// </summary>
        /// <param name="instanceId"></param>
        public OneDriveCore(string instanceId) : base(instanceId)
        {
        }
    }
}
