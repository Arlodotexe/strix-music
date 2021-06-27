using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the track number.
    /// </summary>
    public class TrackNumberComparer<TTrack> : Comparer<TTrack> where TTrack : ITrack
    {
        /// <inheritdoc/>
        public override int Compare(TTrack x, TTrack y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>.
            return Nullable.Compare(x.TrackNumber, y.TrackNumber);
        }
    }
}
