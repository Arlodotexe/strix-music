using StrixMusic.Sdk.Data.Core;
using System.Collections.Generic;

namespace StrixMusic.Sdk.Data.Merged
{
    internal class MergedPlaylist : IMerged<object>
    {
        private List<ICorePlaylist> lists;

        public MergedPlaylist(List<ICorePlaylist> lists)
        {
            this.lists = lists;
        }
    }
}