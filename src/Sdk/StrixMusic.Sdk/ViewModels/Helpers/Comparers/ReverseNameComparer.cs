using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the name in reverse order.
    /// </summary>
    public class ReverseNameComparer : Comparer<TrackViewModel>
    {
        /// <inheritdoc/>
        public override int Compare(TrackViewModel x, TrackViewModel y)
        {
            var result = string.Compare(x.Name, y.Name, false, CultureInfo.CurrentCulture);

            return result == 1 ? result *= -1 : result;
        }
    }
}
