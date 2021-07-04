using System;
using StrixMusic.Sdk.Data.Base;
using StrixMusic.Sdk.ViewModels.Helpers.Comparers.Abstract;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration />.
    /// </summary>
    public class AddedAtComparer<TCollectionITem> : InversableComparer<TCollectionITem> where TCollectionITem : IPlayableCollectionItem
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddedAtComparer{TPlayableBase}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public AddedAtComparer(bool isDescending = false) : base(isDescending)
        {
        }

        /// <inheritdoc/>
        public override int Compare(TCollectionITem x, TCollectionITem y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater, less or equal relation.
            int value = Nullable.Compare(x.AddedAt, y.AddedAt);
            return IsDescending ? -value : value;
        }
    }
}
