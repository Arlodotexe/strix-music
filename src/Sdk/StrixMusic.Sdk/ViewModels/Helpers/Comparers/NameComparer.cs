using System.Collections.Generic;
using System.Globalization;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// A class that can compare names.
    /// </summary>
    public class NameComparer<TTrack> : Comparer<TTrack> where TTrack : ITrack
    {
        /// <inheritdoc/>
        public override int Compare(TTrack x, TTrack y) => string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);
    }
}