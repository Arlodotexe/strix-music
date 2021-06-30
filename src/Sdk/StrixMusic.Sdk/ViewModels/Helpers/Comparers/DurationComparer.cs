using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration/>.
    /// </summary>
    public class DurationComparer<TCollectionITem> : Comparer<TCollectionITem> where TCollectionITem : IPlayableCollectionItem
    {
        /// <inheritdoc/>
        public override int Compare(TCollectionITem x, TCollectionITem y) => TimeSpan.Compare(x.Duration, y.Duration);
    }
}
