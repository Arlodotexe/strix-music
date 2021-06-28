using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using StrixMusic.Sdk.Data;

namespace StrixMusic.Sdk.ViewModels.Helpers.Comparers
{
    /// <summary>
    /// Compares the duration />.
    /// </summary>
    public class AddedAtComparer<TTrack> : Comparer<TTrack> where TTrack : ITrack
    {
        /// <inheritdoc/>
        public override int Compare(TTrack x, TTrack y)
        {
            // Handling nullable dataTypes while comparison using Nullable<T>. It also compares the values of the dataType provided and returns greater,less or equal relation.
            return Nullable.Compare(x.AddedAt, y.AddedAt);
        }
    }
}
