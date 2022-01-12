using System;
using StrixMusic.Sdk.Models.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Comparers.Abstract;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the LastPlayed item/>.
    /// </summary>
    /// <typeparam name="TPlayableBase">The <inheritdoc cref="IPlayableCollectionItem"/> to sort.</typeparam>
    public sealed class LastPlayedComparer<TPlayableBase> : InversableComparer<TPlayableBase>
        where TPlayableBase : IPlayableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LastPlayedComparer{TPlayableBase}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public LastPlayedComparer(bool isDescending = false) : base(isDescending)
        {
        }

        /// <inheritdoc/>
        public override int Compare(TPlayableBase x, TPlayableBase y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater,less or equal relation.
            int value = Nullable.Compare(x.LastPlayed, y.LastPlayed);
            return IsDescending ? -value : value;
        }
    }
}
