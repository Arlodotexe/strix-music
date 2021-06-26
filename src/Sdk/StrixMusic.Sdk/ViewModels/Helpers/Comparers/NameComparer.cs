using System.Collections.Generic;
using System.Globalization;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// A class that can compare names.
    /// </summary>
    public class NameComparer : Comparer<TrackViewModel>
    {
        /// <inheritdoc/>
        public override int Compare(TrackViewModel x, TrackViewModel y)
        {
            return string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);
        }
    }
}