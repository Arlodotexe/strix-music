using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the name in reverse order.
    /// </summary>
    public class ReverseNameComparer<TCollectionITem> : Comparer<TCollectionITem> where TCollectionITem : IPlayableCollectionItem
    {
        /// <inheritdoc/>
        public override int Compare(TCollectionITem x, TCollectionITem y)
        {
            var result = string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);

            return result == 1 ? result *= -1 : result;
        }
    }
}
