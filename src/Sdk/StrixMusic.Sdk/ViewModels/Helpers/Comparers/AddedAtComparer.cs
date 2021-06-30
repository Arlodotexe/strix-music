using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration />.
    /// </summary>
    public class AddedAtComparer<TCollectionITem> : Comparer<TCollectionITem> where TCollectionITem : IPlayableCollectionItem
    {
        /// <inheritdoc/>
        public override int Compare(TCollectionITem x, TCollectionITem y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater,less or equal relation.
            return Nullable.Compare(x.AddedAt, y.AddedAt);
        }
    }
}
