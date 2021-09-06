﻿using StrixMusic.Core.LocalFiles.Models;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Core.OneDriveCore.Models
{
    /// <summary>
    /// The recently played items for the <see cref="OneDriveCore"/>.
    /// </summary>
    public class OneDriveCoreRecentlyPlayed : LocalFilesCoreRecentlyPlayed
    {
        /// <inheritdoc />
        public OneDriveCoreRecentlyPlayed(ICore sourceCore)
            : base(sourceCore)
        {
        }
    }
}