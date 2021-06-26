using System.Collections.Generic;
using System.Globalization;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// A class that can compare names.
    /// </summary>
    public class NameComparision<TTrack> : Comparer<ITrack> where TTrack : ITrack
    {
        /// <inheritdoc/>
        public override int Compare(ITrack x, ITrack y)
        {
            return string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);
        }
    }
}