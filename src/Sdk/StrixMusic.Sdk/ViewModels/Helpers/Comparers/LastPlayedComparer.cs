using System;
using System.Collections.Generic;
using System.Text;
using StrixMusic.Sdk.Data.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the LastPlayed item/>.
    /// </summary>
    /// <typeparam name="TPlayableBase">The <inheritdoc cref="IPlayableCollectionItem"/> to sort.</typeparam>
    public class LastPlayedComparer<TPlayableBase> : Comparer<TPlayableBase> where TPlayableBase : IPlayableBase
    {
        /// <inheritdoc/>
        public override int Compare(TPlayableBase x, TPlayableBase y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater,less or equal relation.
            return Nullable.Compare(x.LastPlayed, y.LastPlayed);
        }
    }
}
