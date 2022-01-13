using System;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration/>.
    /// </summary>
    public sealed class DurationComparer<TPlayableBase> : InversableComparer<TPlayableBase>
        where TPlayableBase : IPlayableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DurationComparer{TPlayableBase}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public DurationComparer(bool isDescending = false) : base(isDescending)
        {
        }

        /// <inheritdoc/>
        public override int Compare(TPlayableBase x, TPlayableBase y)
        {
            int value = TimeSpan.Compare(x.Duration, y.Duration);
            return IsDescending ? -value : value;
        }
    }
}
