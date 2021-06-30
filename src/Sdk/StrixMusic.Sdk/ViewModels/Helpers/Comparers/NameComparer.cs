using System.Collections.Generic;
using System.Globalization;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// A class that can compare names.
    /// </summary>
    public class NameComparer<TCollectionITem> : Comparer<TCollectionITem> where TCollectionITem : IPlayableCollectionItem
    {
        /// <inheritdoc/>
        public override int Compare(TCollectionITem x, TCollectionITem y) => string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);
    }
}