using System.Globalization;
using StrixMusic.Sdk.Models.Base;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// A class that can compare names.
    /// </summary>
    public sealed class NameComparer<TPlayableBase> : InversableComparer<TPlayableBase>
        where TPlayableBase : IPlayableBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NameComparer{TPlayableBase}"/> class.
        /// </summary>
        /// <param name="isDescending">Sets if the comparer operates in descending order.</param>
        public NameComparer(bool isDescending = false) : base(isDescending)
        {
        }

        /// <inheritdoc/>
        public override int Compare(TPlayableBase x, TPlayableBase y)
        {
            int value = string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);
            return IsDescending ? -value : value;
        }
    }
}