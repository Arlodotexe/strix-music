using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration/>.
    /// </summary>
    public class DurationComparer<TTrack> : Comparer<TTrack> where TTrack : ITrack
    {
        /// <inheritdoc/>
        public override int Compare(TTrack x, TTrack y) => TimeSpan.Compare(x.Duration, y.Duration);
    }
}
