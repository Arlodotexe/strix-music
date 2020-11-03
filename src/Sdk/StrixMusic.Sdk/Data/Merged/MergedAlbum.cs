using System.Collections.Generic;
using StrixMusic.Sdk.Data.Core;

namespace StrixMusic.Sdk.Data.Merged
{
    public class MergedAlbum : IAlbum, IMerged<ICoreAlbum>
    {
        /// <summary>
        /// Creates a new instance of <see cref="MergedAlbum"/>.
        /// </summary>
        /// <param name="sources"></param>
        public MergedAlbum(IReadOnlyList<ICoreAlbum> sources)
        {

        }

        
    }
}